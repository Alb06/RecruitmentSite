using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace Recrut.API.Middleware
{
    public class DatabaseConnectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseConnectionMiddleware> _logger;

        public DatabaseConnectionMiddleware(RequestDelegate next, ILogger<DatabaseConnectionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (IsDatabaseConnectionError(ex))
            {
                await HandleDatabaseConnectionErrorAsync(context, ex);
            }
        }

        private static bool IsDatabaseConnectionError(Exception exception)
        {
            return exception switch
            {
                NpgsqlException npgsqlEx => npgsqlEx.InnerException is SocketException,
                InvalidOperationException invalidOpEx => invalidOpEx.Message.Contains("transient failure"),
                DbUpdateException dbEx => dbEx.InnerException is NpgsqlException,
                _ => false
            };
        }

        private async Task HandleDatabaseConnectionErrorAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception,
                "\n🚨 Database connection error: {Message} \n" +
                "Please ensure PostgreSQL is running and accessible on 5432 docker machine port.\n",
                exception.Message);

            context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            context.Response.ContentType = "application/json";

            var response = new
            {
                Success = false,
                Message = "Service temporairement indisponible. La base de données n'est pas accessible.",
                Details = "Vérifiez que PostgreSQL est démarré et accessible sur le port 5432.",
                Timestamp = DateTime.UtcNow,
                TraceId = context.TraceIdentifier
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}