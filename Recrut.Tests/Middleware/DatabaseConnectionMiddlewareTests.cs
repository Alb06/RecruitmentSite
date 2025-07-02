using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using Recrut.API.Middleware;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace Recrut.TestU.Middleware
{
    public class DatabaseConnectionMiddlewareTests
    {
        private readonly Mock<ILogger<DatabaseConnectionMiddleware>> _mockLogger;
        private readonly DatabaseConnectionMiddleware _middleware;

        public DatabaseConnectionMiddlewareTests()
        {
            _mockLogger = new Mock<ILogger<DatabaseConnectionMiddleware>>();

            // Mock du delegate suivant qui lève une exception
            RequestDelegate next = (HttpContext context) =>
                throw new NpgsqlException("Connection failed", new SocketException(111));

            _middleware = new DatabaseConnectionMiddleware(next, _mockLogger.Object);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnServiceUnavailable_WhenDatabaseConnectionFails()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            // Act
            await _middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.ServiceUnavailable, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);

            // Vérifier le contenu de la réponse
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var response = JsonSerializer.Deserialize<dynamic>(responseBody);

            Assert.NotNull(response);
        }
    }
}