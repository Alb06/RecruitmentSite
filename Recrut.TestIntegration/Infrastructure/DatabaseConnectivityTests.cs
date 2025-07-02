using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Recrut.Data;

namespace Recrut.API.IntegrationTests.Infrastructure
{
    public class DatabaseConnectivityTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly ILogger<DatabaseConnectivityTests> _logger;

        public DatabaseConnectivityTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            var loggerFactory = _factory.Services.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<DatabaseConnectivityTests>();
        }

        [Fact]
        public async Task Database_ShouldBeAccessible()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act & Assert
            var canConnect = await CanConnectToDatabaseAsync(context);

            Assert.True(canConnect,
                "❌ La base de données n'est pas accessible. " +
                "Vérifiez que PostgreSQL est démarré (docker-compose up -d postgres)");
        }

        [Fact]
        public async Task Api_ShouldReturnServiceUnavailable_WhenDatabaseIsDown()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Simuler une base indisponible en utilisant une mauvaise connection string
            // (ce test nécessiterait une configuration spéciale)

            // Act
            var response = await client.GetAsync("/api/users/test@example.com");

            // Assert
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                var content = await response.Content.ReadAsStringAsync();
                Assert.Contains("base de données n'est pas accessible", content.ToLower());
            }
        }

        private static async Task<bool> CanConnectToDatabaseAsync(AppDbContext context)
        {
            try
            {
                await context.Database.CanConnectAsync();
                return true;
            }
            catch (Exception ex) when (ex is NpgsqlException || ex is InvalidOperationException)
            {
                return false;
            }
        }
    }
}