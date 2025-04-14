using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Recrut.Business.Services.Interfaces;
using Recrut.Data;

namespace Recrut.API.IntegrationTests
{
    /// <summary>
    /// Fabrique personnalisée pour créer une instance d'application web pour les tests d'intégration.
    /// Cette classe configure l'application pour utiliser une base de données en mémoire et initialise
    /// les données de test nécessaires.
    /// </summary>
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Identifier et supprimer l'enregistrement du DbContext réel
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Ajouter SQLite en mémoire pour les tests au lieu de InMemoryDatabase
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open(); // Important: la connexion doit rester ouverte pour conserver la DB en mémoire

                // Ajouter le DbContext en mémoire pour les tests
                // UseInMemoryDatabase pour éviter la vérification des clés étrangères en SQLite
                // Sinon switch avec SQLite
                services.AddDbContext<AppDbContext>(options =>
                {
                    //options.UseInMemoryDatabase("RecrutTestDb");
                    options.UseSqlite(connection);
                });

                services.AddScoped<IPasswordHasher, TestPasswordHasher>();

                // Construire le fournisseur de services
                var sp = services.BuildServiceProvider();

                // Créer un scope pour obtenir les services nécessaires
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

                    // Assurer que la base de données est créée
                    db.Database.EnsureCreated();

                    try
                    {
                        // Initialiser la base de données avec des données de test
                        TestDataSeeder.InitializeDbForTests(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Une erreur s'est produite lors de l'initialisation de la base de données de test.");
                    }
                }
            });
        }
    }

    /// <summary>
    /// Implémentation simplifiée du service de hachage de mot de passe pour les tests.
    /// Utilise un algorithme déterministe pour faciliter les tests.
    /// </summary>
    public class TestPasswordHasher : IPasswordHasher
    {
        private const string KnownPasswordHash = "MLJHHujkYjG4yUY+gVP0Xtd2fYc4TyiQrsB1RMMxcQA=:WQvLvSMg5k7nDYGz:10000:SHA256";

        public string HashPassword(string password)
        {
            return KnownPasswordHash;
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return password == "Password123!" && hashedPassword == KnownPasswordHash;
        }
    }
}