using Microsoft.Extensions.Options;
using Moq;
using Recrut.Business.Services;
using Recrut.Business.Services.Interfaces;
using Recrut.Data.Repositories.Interfaces;
using Recrut.Models;
using Recrut.Shared.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace Recrut.Tests.Services
{
    /// <summary>
    /// Tests unitaires pour le service AuthService
    /// </summary>
    public class AuthServiceTests
    {
        private readonly Mock<IUserAuthRepository> _mockUserAuthRepository;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly JwtOptions _jwtOptions;
        private readonly AuthService _authService;

        private readonly int testUserId = 2;
        private readonly int testUserNotFoundId = 3;

        public AuthServiceTests()
        {
            _mockUserAuthRepository = new Mock<IUserAuthRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();

            // Configuration JWT pour les tests
            // Important: La clé secrète doit être suffisamment longue pour JWT
            _jwtOptions = new JwtOptions
            {
                Secret = "test_secret_key_for_testing_purposes_only_must_be_at_least_256_bits_long",
                Issuer = "test_issuer",
                Audience = "test_audience",
                ExpiryMinutes = 60
            };

            _authService = new AuthService(
                _mockUserAuthRepository.Object,
                Options.Create(_jwtOptions),
                _mockPasswordHasher.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnSuccessAndValidToken()
        {
            // Arrange
            var email = "test@example.com";
            var password = "hashed_password";
            var user = new User
            {
                Id = testUserId,
                Email = email,
                Name = "Unit Test User",
                PasswordHash = "hashed_password",
                Roles = new List<Role> { new Role { Id = testUserId, Name = "User" } }
            };

            _mockUserAuthRepository.Setup(repo => repo.GetUserByEmailWithRolesAsync(email))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword(password, user.PasswordHash))
                .Returns(true);

            // Act
            var result = await _authService.AuthenticateAsync(email, password);

            // Assert
            Assert.True(result.Success);
            Assert.NotEmpty(result.Token);

            // Vérification basique du format JWT (commence par eyJ)
            Assert.StartsWith("eyJ", result.Token);

            // Vérification plus avancée avec JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();
            Assert.True(tokenHandler.CanReadToken(result.Token));

            var token = tokenHandler.ReadJwtToken(result.Token);
            Assert.Equal(_jwtOptions.Issuer, token.Issuer);
            Assert.Equal(_jwtOptions.Audience, token.Audiences.First());
            Assert.Equal(user.Id.ToString(), token.Subject);
            Assert.Equal(user.Email, token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
            Assert.Contains(token.Claims, c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "User");
        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidEmail_ShouldReturnFailure()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var password = "password";

            _mockUserAuthRepository.Setup(repo => repo.GetUserByEmailWithRolesAsync(email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.AuthenticateAsync(email, password);

            // Assert
            Assert.False(result.Success);
            Assert.Empty(result.Token);
        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidPassword_ShouldReturnFailure()
        {
            // Arrange
            var email = "test@example.com";
            var password = "wrong_password";
            var user = new User
            {
                Id = testUserId,
                Name = "Test User",
                Email = email,
                PasswordHash = "hashed_password",
                Roles = new List<Role> { new Role { Id = testUserId, Name = "User" } }
            };

            _mockUserAuthRepository.Setup(repo => repo.GetUserByEmailWithRolesAsync(email))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword(password, user.PasswordHash))
                .Returns(false);

            // Act
            var result = await _authService.AuthenticateAsync(email, password);

            // Assert
            Assert.False(result.Success);
            Assert.Empty(result.Token);
        }

        [Fact]
        public async Task IsInRoleAsync_UserHasRole_ShouldReturnTrue()
        {
            // Arrange
            int userId = testUserId;
            string roleName = "Admin";

            _mockUserAuthRepository.Setup(repo => repo.GetUserRoleNamesAsync(userId))
                .ReturnsAsync(new List<string> { "User", "Admin" });

            // Act
            var result = await _authService.IsInRoleAsync(userId, roleName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsInRoleAsync_UserDoesNotHaveRole_ShouldReturnFalse()
        {
            // Arrange
            int userId = testUserId;
            string roleName = "SuperAdmin";

            _mockUserAuthRepository.Setup(repo => repo.GetUserRoleNamesAsync(userId))
                .ReturnsAsync(new List<string> { "User", "Admin" });

            // Act
            var result = await _authService.IsInRoleAsync(userId, roleName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsInRoleAsync_UserNotFound_ShouldReturnFalse()
        {
            // Arrange
            int userId = testUserNotFoundId;
            string roleName = "Admin";

            _mockUserAuthRepository.Setup(repo => repo.GetUserRoleNamesAsync(userId))
                .ReturnsAsync(Enumerable.Empty<string>());

            // Act
            var result = await _authService.IsInRoleAsync(userId, roleName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetUserRolesAsync_UserHasRoles_ShouldReturnRoles()
        {
            // Arrange
            int userId = testUserId;
            var expectedRoles = new List<string> { "User", "Admin" };

            _mockUserAuthRepository.Setup(repo => repo.GetUserRoleNamesAsync(userId))
                .ReturnsAsync(expectedRoles);

            // Act
            var result = await _authService.GetUserRolesAsync(userId);

            // Assert
            Assert.Equal(expectedRoles, result);
        }

        [Fact]
        public async Task GetUserRolesAsync_UserHasNoRoles_ShouldReturnEmptyList()
        {
            // Arrange
            int userId = testUserId;

            _mockUserAuthRepository.Setup(repo => repo.GetUserRoleNamesAsync(userId))
                .ReturnsAsync(new List<string>());

            // Act
            var result = await _authService.GetUserRolesAsync(userId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserRolesAsync_UserNotFound_ShouldReturnEmptyList()
        {
            // Arrange
            int userId = testUserNotFoundId;

            _mockUserAuthRepository.Setup(repo => repo.GetUserRoleNamesAsync(userId))
                .ReturnsAsync(Enumerable.Empty<string>());

            // Act
            var result = await _authService.GetUserRolesAsync(userId);

            // Assert
            Assert.Empty(result);
        }
    }
}