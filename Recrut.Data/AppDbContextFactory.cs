using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Recrut.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Remplacez la chaîne de connexion par la vôtre.
            optionsBuilder.UseNpgsql("Host=host.docker.internal;Port=5432;Database=recrutdb;Username=zahagadmin;Password=24rnUZ42");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
