using Recrut.Data;
using Recrut.Models;

namespace Recrut.API.IntegrationTests
{
    /// <summary>
    /// Classe utilitaire pour initialiser les données de test dans la base de données
    /// </summary>
    public static class TestDataSeeder
    {
        // Mot de passe haché connu pour les tests
        public const string KnownPasswordHash = "MLJHHujkYjG4yUY+gVP0Xtd2fYc4TyiQrsB1RMMxcQA=:WQvLvSMg5k7nDYGz:10000:SHA256";

        /// <summary>
        /// Initialise la base de données avec des données de test
        /// </summary>
        /// <param name="context">Le contexte de base de données</param>
        public static void InitializeDbForTests(AppDbContext context)
        {
            // Nettoyer les données existantes
            context.Users.RemoveRange(context.Users);
            context.Set<Role>().RemoveRange(context.Set<Role>());
            context.SaveChanges();

            // Créer les rôles
            var adminRole = new Role { Id = 1, Name = "Admin" };
            var userRole = new Role { Id = 2, Name = "User" };
            context.Set<Role>().AddRange(adminRole, userRole);

            // Créer les utilisateurs avec leurs rôles associés
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "Admin User",
                    Email = "admin@example.com",
                    PasswordHash = KnownPasswordHash,
                    Roles = new List<Role> { adminRole }
                },
                new User
                {
                    Id = 2,
                    Name = "Regular User",
                    Email = "user@example.com",
                    PasswordHash = KnownPasswordHash,
                    Roles = new List<Role> { userRole }
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}