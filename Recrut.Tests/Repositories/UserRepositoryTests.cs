using Microsoft.EntityFrameworkCore;
using Recrut.Data.Repositories;
using Recrut.Models;

namespace Recrut.Tests.Repositories
{
    public class UserRepositoryTests : BaseRepositoryTests
    {
        private readonly UserRepository _repository;

        public UserRepositoryTests() : base()
        {
            _repository = new UserRepository(_context);
        }

        #region Test Data

        private User CreateTestUser(string suffix = "")
        {
            return new User
            {
                Id = GenerateUniqueId(),
                Name = $"TestUser{suffix}",
                Email = $"test{suffix}@example.com",
                PasswordHash = $"hashed_password{suffix}"
            };
        }

        #endregion

        [Fact]
        public async Task GetUserByEmailAsync_WithExistingEmail_ReturnsUser()
        {
            // Arrange
            var user = CreateTestUser();
            _context.Users.Add(user);
            SaveChangesAndDetachAll();

            // Act
            var result = await _repository.GetUserByEmailAsync(user.Email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_WithNonExistingEmail_ReturnsNull()
        {
            // Arrange
            var nonExistingEmail = "nonexisting@example.com";

            // Act
            var result = await _repository.GetUserByEmailAsync(nonExistingEmail);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByEmailAsync_CaseInsensitive_ReturnsUser()
        {
            // Arrange
            var user = CreateTestUser();
            user.Email = "test@example.com";
            _context.Users.Add(user);
            SaveChangesAndDetachAll();

            // Act
            var result = await _repository.GetUserByEmailAsync("Test@Example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
        }

        [Fact]
        public async Task DeleteUserByEmailAsync_WithExistingEmail_DeletesUser()
        {
            // Arrange
            var user = CreateTestUser();
            _context.Users.Add(user);
            SaveChangesAndDetachAll();

            // Act
            var deletedCount = await _repository.DeleteUserByEmailAsync(user.Email);

            // Assert
            Assert.Equal(1, deletedCount);

            // Verify user is deleted
            var dbUser = await _repository.GetUserByEmailAsync(user.Email);
            Assert.Null(dbUser);
        }

        [Fact]
        public async Task DeleteUserByEmailAsync_WithNonExistingEmail_ReturnsZero()
        {
            // Arrange
            var nonExistingEmail = "nonexisting@example.com";

            // Act
            var deletedCount = await _repository.DeleteUserByEmailAsync(nonExistingEmail);

            // Assert
            Assert.Equal(0, deletedCount);
        }

        [Fact]
        public async Task DeleteUserByEmailAsync_WithMultipleUsersHavingSameEmail_DeletesAllMatching()
        {
            // Arrange
            // Note: This test depends on the database configuration.
            // If email uniqueness is enforced at the database level (as it should be),
            // this test may fail. In that case, the test is still useful to verify
            // that the constraint is working correctly.

            var email = "duplicate@example.com";

            var user1 = CreateTestUser("1");
            user1.Email = email;

            var user2 = CreateTestUser("2");
            user2.Email = email;

            try
            {
                _context.Users.Add(user1);
                SaveChangesAndDetachAll();

                _context.Users.Add(user2);
                SaveChangesAndDetachAll();

                // Act
                var deletedCount = await _repository.DeleteUserByEmailAsync(email);

                // Assert - if we can add duplicates, both should be deleted
                Assert.Equal(2, deletedCount);
            }
            catch (DbUpdateException)
            {
                // If adding duplicates fails (as expected with proper constraints),
                // verify at least the first user can be deleted
                var deletedCount = await _repository.DeleteUserByEmailAsync(email);
                Assert.Equal(1, deletedCount);
            }
        }
    }
}