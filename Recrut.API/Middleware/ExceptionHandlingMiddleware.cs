using Microsoft.EntityFrameworkCore;
using Npgsql;
using Recrut.API.DTOs;
using Recrut.API.Enums;
using Recrut.Models;
using System.Net;
using System.Text.Json;

namespace Recrut.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            string message = "Une erreur s'est produite.";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;

            switch (exception)
            {
                case DbUpdateException dbUpdateEx when dbUpdateEx.InnerException is PostgresException pgEx && pgEx.SqlState == ((int)PGEnums.Execeptions.UniqueViolation).ToString():
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    message = IdentifyUniqueConstraintViolation(pgEx);
                    _logger.LogError(exception, $"Violation de contrainte d'unicité détectée - {pgEx.ConstraintName}.");
                    break;

                default:
                    _logger.LogError(exception, "Exception non gérée détectée.");
                    break;
            }

            var result = JsonSerializer.Serialize(new OperationResult
            {
                Success = false,
                Message = message
            });

            await context.Response.WriteAsync(result);
        }

        private string IdentifyUniqueConstraintViolation(PostgresException pgEx)
        {
            switch (pgEx.ConstraintName)
            {
                case string constraintName when constraintName.Contains(nameof(User.Name)):
                    return "Le nom est déjà utilisé.";
                case string constraintName when constraintName.Contains(nameof(User.Email)):
                    return "L'adresse email est déjà utilisée.";
                default:
                    return "Une contrainte d'unicité a été violée.";
            }
        }
    }
}

