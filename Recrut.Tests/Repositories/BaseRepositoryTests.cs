using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Recrut.Data;
using System.Data.Common;

namespace Recrut.Tests.Repositories
{
    /// <summary>
    /// Classe de base pour les tests de repository
    /// Fournit une configuration SQLite en mémoire pour tester les repositories
    /// </summary>
    public abstract class BaseRepositoryTests : IDisposable
    {
        private readonly DbConnection _connection;
        protected readonly AppDbContext _context;

        protected BaseRepositoryTests()
        {
            // Création d'une connexion SQLite en mémoire
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            // Configuration du DbContext avec SQLite
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Création du contexte et de la base de données
            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
        }

        /// <summary>
        /// Génère un ID unique pour les tests
        /// </summary>
        protected int GenerateUniqueId()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode() % 1000000);
        }

        /// <summary>
        /// Sauvegarde les modifications dans le contexte et détache les entités
        /// pour simuler des opérations distinctes comme en production
        /// </summary>
        protected void SaveChangesAndDetachAll()
        {
            _context.SaveChanges();

            // Détacher toutes les entités suivies
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }

        /// <summary>
        /// Nettoyage des ressources
        /// </summary>
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _connection.Dispose();
        }
    }
}