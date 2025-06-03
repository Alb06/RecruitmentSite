using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Recrut.Data;

namespace Recrut.API.Configuration
{
    public static class DatabaseConfiguration
    {
        public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "The connection string 'DefaultConnection' is not configured. " +
                    "Please configure User Secrets in development or environment variables in production.");
            }

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString)
            );
        }
    }
}
