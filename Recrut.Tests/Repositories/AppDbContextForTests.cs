using Microsoft.EntityFrameworkCore;
using Recrut.Data;

namespace Recrut.TestU.Repositories
{
    /// <summary>
    /// Cette classe étend AppDbContext pour adapter son comportement aux tests
    /// </summary>
    public class AppDbContextForTests : AppDbContext
    {
        public AppDbContextForTests(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Adapter les configurations spécifiques à PostgreSQL pour SQLite

            // Par exemple, pour les propriétés qui utilisent UseIdentityByDefaultColumn:
            // Vous pouvez remplacer par des alternatives SQLite si nécessaire

            // Si vous avez des contraintes ou fonctionnalités spécifiques à PostgreSQL,
            // vous pouvez les adapter ici pour SQLite

            // Exemple:
            // modelBuilder.Entity<User>()
            //    .Property(u => u.Id)
            //    .ValueGeneratedOnAdd();
        }
    }
}
