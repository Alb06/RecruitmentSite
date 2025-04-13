using Recrut.Data.Repositories;
using Recrut.Models;

namespace Recrut.Tests.Repositories
{
    public class UserAuthRepositoryTests : BaseRepositoryTests
    {
        private readonly UserAuthRepository _repository;

        public UserAuthRepositoryTests() : base()
        {
            _repository = new UserAuthRepository(_context);
        }

        #region Test Data

        private User CreateUserWithRoles(string suffix = "", string[] roleNames = null)
        {
            var user = new User
            {
                Id = GenerateUniqueId(),
                Name = $"TestUser{suffix}",
                Email = $"test{suffix}@example.com",
                PasswordHash = $"hashed_password{suffix}"
            };

            if (roleNames != null && roleNames.Length > 0)
            {
                foreach (var roleName in roleNames)
                {
                    var role = _context.Set<Role>().FirstOrDefault(r => r.Name == roleName);

                    if (role == null)
                    {
                        role = new Role
                        {
                            Id = GenerateUniqueId(),
                            Name = roleName
                        };
                        _context.Set<Role>().Add(role);
                    }

                    user.Roles.Add(role);
                }
            }

            return user;
        }

        private void SeedDefaultRoles()
        {
            // Seed les mêmes rôles que dans AppDbContext.OnModelCreating
            if (!_context.Set<Role>().Any())
            {
                _context.Set<Role>().AddRange(
                    new Role { Id = 1, Name = "Admin" },
                    new Role { Id = 2, Name = "User" }
                );
                SaveChangesAndDetachAll();
            }
        }

        #endregion

        [Fact]
        public async Task GetUserByEmailWithRolesAsync_WithExistingEmail_ReturnsUserWithRoles()
        {
            // Arrange
            SeedDefaultRoles();
            var user = CreateUserWithRoles("1", new[] { "Admin", "User" });
            _context.Users.Add(user);
            SaveChangesAndDetachAll();

            // Act
            var result = await _repository.GetUserByEmailWithRolesAsync(user.Email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email, result.Email);

            // Verify roles
            Assert.NotNull(result.Roles);
            Assert.Equal(2, result.Roles.Count);
            Assert.Contains(result.Roles, r => r.Name == "Admin");
            Assert.Contains(result.Roles, r => r.Name == "User");
        }

        [Fact]
        public async Task GetUserByEmailWithRolesAsync_WithNonExistingEmail_ReturnsNull()
        {
            // Arrange
            var nonExistingEmail = "nonexisting@example.com";

            // Act
            var result = await _repository.GetUserByEmailWithRolesAsync(nonExistingEmail);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByIdWithRolesAsync_WithExistingId_ReturnsUserWithRoles()
        {
            // Arrange
            SeedDefaultRoles();
            var user = CreateUserWithRoles("1", new[] { "Admin" });
            _context.Users.Add(user);
            SaveChangesAndDetachAll();

            // Act
            var result = await _repository.GetUserByIdWithRolesAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);

            // Verify roles
            Assert.NotNull(result.Roles);
            Assert.Single(result.Roles);
            Assert.Contains(result.Roles, r => r.Name == "Admin");
        }

        [Fact]
        public async Task GetUserByIdWithRolesAsync_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            var nonExistingId = -1;

            // Act
            var result = await _repository.GetUserByIdWithRolesAsync(nonExistingId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserRoleNamesAsync_WithExistingUserHavingRoles_ReturnsRoleNames()
        {
            // Arrange
            SeedDefaultRoles();
            var user = CreateUserWithRoles("1", new[] { "Admin", "User" });
            _context.Users.Add(user);
            SaveChangesAndDetachAll();

            // Act
            var roleNames = await _repository.GetUserRoleNamesAsync(user.Id);

            // Assert
            Assert.NotNull(roleNames);
            Assert.Equal(2, roleNames.Count());
            Assert.Contains("Admin", roleNames);
            Assert.Contains("User", roleNames);
        }

        [Fact]
        public async Task GetUserRoleNamesAsync_WithExistingUserHavingNoRoles_ReturnsEmptyCollection()
        {
            // Arrange
            var user = CreateUserWithRoles("1");
            _context.Users.Add(user);
            SaveChangesAndDetachAll();

            // Act
            var roleNames = await _repository.GetUserRoleNamesAsync(user.Id);

            // Assert
            Assert.NotNull(roleNames);
            Assert.Empty(roleNames);
        }

        [Fact]
        public async Task GetUserRoleNamesAsync_WithNonExistingUser_ReturnsEmptyCollection()
        {
            // Arrange
            var nonExistingId = -1;

            // Act
            var roleNames = await _repository.GetUserRoleNamesAsync(nonExistingId);

            // Assert
            Assert.NotNull(roleNames);
            Assert.Empty(roleNames);
        }
    }
}