using Microsoft.EntityFrameworkCore;
using Recrut.Data;

namespace Recrut.API.Configuration
{
    public static class DatabaseConfiguration
    {
        public static void AddDatabaseConfiguration(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql("Host=host.docker.internal;Port=5432;Database=recrutdb;Username=zahagadmin;Password=24rnUZ42")
            );
        }
    }
}
