using Recrut.Data.Repositories;
using Recrut.Models;

namespace Recrut.TestU.Repositories
{
    public class GenericRepositoryTests : BaseRepositoryTests
    {
        private readonly Repository<User> _repository;

        public GenericRepositoryTests() : base()
        {
            _repository = new Repository<User>(_context);
        }

        #region Test Data

        private static User CreateTestUser(string suffix = "")
        {
            return new User
            {
                Id = GenerateUniqueId(),
                Name = $"TestUser{suffix}",
                Email = $"test{suffix}@example.com",
                PasswordHash = $"hashed_password{suffix}"
            };
        }

        private List<User> SeedUsers(int count)
        {
            var users = new List<User>();
            for (int i = 0; i < count; i++)
            {
                var user = CreateTestUser(i.ToString());
                users.Add(user);
                _context.Users.Add(user);
            }
            SaveChangesAndDetachAll();
            return users;
        }

        #endregion

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsEntity()
        {
            // Arrange
            var user = CreateTestUser();
            _context.Users.Add(user);
            SaveChangesAndDetachAll();

            // Act
            var result = await _repository.GetByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            var nonExistingId = -1;

            // Act
            var result = await _repository.GetByIdAsync(nonExistingId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdsAsync_WithExistingIds_ReturnsEntities()
        {
            // Arrange
            var users = SeedUsers(3);
            var ids = users.Select(u => u.Id).ToList();

            // Act
            var results = await _repository.GetByIdsAsync(ids);

            // Assert
            Assert.NotNull(results);
            Assert.Equal(users.Count, results.Count());

            foreach (var user in users)
            {
                Assert.Contains(results, u => u.Id == user.Id);
            }
        }

        [Fact]
        public async Task GetByIdsAsync_WithSomeNonExistingIds_ReturnsOnlyExistingEntities()
        {
            // Arrange
            var users = SeedUsers(2);
            var ids = new List<int> { users[0].Id, users[1].Id, -1 };

            // Act
            var results = await _repository.GetByIdsAsync(ids);

            // Assert
            Assert.NotNull(results);
            Assert.Equal(2, results.Count());

            foreach (var user in users)
            {
                Assert.Contains(results, u => u.Id == user.Id);
            }
        }

        [Fact]
        public async Task GetByAsync_WithMatchingPredicate_ReturnsEntities()
        {
            // Arrange
            SeedUsers(5);
            var specialUser = CreateTestUser("Special");
            specialUser.Email = "special@example.com";
            _context.Users.Add(specialUser);
            SaveChangesAndDetachAll();

            // Act
            var results = await _repository.GetByAsync(u => u.Email == "special@example.com");

            // Assert
            Assert.NotNull(results);
            Assert.Single(results);
            Assert.Equal(specialUser.Id, results.First().Id);
        }

        [Fact]
        public async Task GetByAsync_WithNonMatchingPredicate_ReturnsEmptyCollection()
        {
            // Arrange
            SeedUsers(3);

            // Act
            var results = await _repository.GetByAsync(u => u.Email == "nonexistent@example.com");

            // Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            // Arrange
            var users = SeedUsers(3);

            // Act
            var results = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(results);
            Assert.Equal(users.Count, results.Count());

            foreach (var user in users)
            {
                Assert.Contains(results, u => u.Id == user.Id);
            }
        }

        [Fact]
        public async Task CreateAsync_AddsEntitiesToDatabase()
        {
            // Arrange
            var users = new List<User>
            {
                CreateTestUser("1"),
                CreateTestUser("2")
            };

            // Act
            await _repository.CreateAsync(users);

            // Assert
            foreach (var user in users)
            {
                var dbUser = await _context.Users.FindAsync(user.Id);
                Assert.NotNull(dbUser);
                Assert.Equal(user.Name, dbUser.Name);
                Assert.Equal(user.Email, dbUser.Email);
            }
        }

        [Fact]
        public async Task UpdateAsync_ModifiesExistingEntity()
        {
            // Arrange
            var user = CreateTestUser();
            _context.Users.Add(user);
            SaveChangesAndDetachAll();

            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);
            updatedUser.Name = "Updated Name";
            updatedUser.Email = "updated@example.com";

            // Act
            await _repository.UpdateAsync(updatedUser);

            // Assert
            var dbUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(dbUser);
            Assert.Equal("Updated Name", dbUser.Name);
            Assert.Equal("updated@example.com", dbUser.Email);
        }

        [Fact]
        public async Task DeleteByIdsAsync_RemovesEntitiesByIds()
        {
            // Arrange
            var users = SeedUsers(3);
            var idsToDelete = users.Take(2).Select(u => u.Id).ToList();

            // Act
            var deletedCount = await _repository.DeleteByIdsAsync(idsToDelete);

            // Assert
            Assert.Equal(2, deletedCount);

            foreach (var id in idsToDelete)
            {
                var dbUser = await _context.Users.FindAsync(id);
                Assert.Null(dbUser);
            }

            // Verify the other user still exists
            var remainingUser = await _context.Users.FindAsync(users[2].Id);
            Assert.NotNull(remainingUser);
        }

        [Fact]
        public async Task DeleteAsync_RemovesEntities()
        {
            // Arrange
            var users = SeedUsers(3);
            var usersToDelete = users.Take(2).ToList();

            // Act
            await _repository.DeleteAsync(usersToDelete);

            // Assert
            foreach (var user in usersToDelete)
            {
                var dbUser = await _context.Users.FindAsync(user.Id);
                Assert.Null(dbUser);
            }

            // Verify the other user still exists
            var remainingUser = await _context.Users.FindAsync(users[2].Id);
            Assert.NotNull(remainingUser);
        }
    }
}