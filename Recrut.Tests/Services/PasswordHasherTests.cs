using Recrut.Business.Services;

namespace Recrut.Tests.Services
{
    /// <summary>
    /// Tests unitaires pour le service PasswordHasher
    /// </summary>
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _passwordHasher;

        public PasswordHasherTests()
        {
            _passwordHasher = new PasswordHasher();
        }

        [Fact]
        public void HashPassword_ShouldReturnFormattedHashString()
        {
            // Arrange
            var password = "TestPassword123";

            // Act
            string hashedPassword = _passwordHasher.HashPassword(password);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.NotEmpty(hashedPassword);

            // Vérification du format attendu: salt:hash:iterations:algorithm
            string[] parts = hashedPassword.Split(':');
            Assert.Equal(4, parts.Length);

            // Vérifier que chaque partie n'est pas vide
            Assert.NotEmpty(parts[0]); // salt
            Assert.NotEmpty(parts[1]); // hash
            Assert.NotEmpty(parts[2]); // iterations
            Assert.Equal("SHA256", parts[3]); // algorithme
        }

        [Fact]
        public void HashPassword_SamePassword_ShouldReturnDifferentHashes()
        {
            // Arrange
            var password = "TestPassword123";

            // Act
            string hashedPassword1 = _passwordHasher.HashPassword(password);
            string hashedPassword2 = _passwordHasher.HashPassword(password);

            // Assert
            // Les hachages doivent être différents en raison du sel aléatoire
            Assert.NotEqual(hashedPassword1, hashedPassword2);
        }

        [Fact]
        public void HashPassword_DifferentPasswords_ShouldReturnDifferentHashes()
        {
            // Arrange
            var password1 = "TestPassword123";
            var password2 = "DifferentPassword456";

            // Act
            string hashedPassword1 = _passwordHasher.HashPassword(password1);
            string hashedPassword2 = _passwordHasher.HashPassword(password2);

            // Assert
            Assert.NotEqual(hashedPassword1, hashedPassword2);
        }

        [Fact]
        public void VerifyPassword_CorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            var password = "TestPassword123";
            string hashedPassword = _passwordHasher.HashPassword(password);

            // Act
            bool result = _passwordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_IncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            var correctPassword = "TestPassword123";
            var incorrectPassword = "WrongPassword456";
            string hashedPassword = _passwordHasher.HashPassword(correctPassword);

            // Act
            bool result = _passwordHasher.VerifyPassword(incorrectPassword, hashedPassword);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("")]  // Chaîne vide
        [InlineData("InvalidHash")] // Format incorrect
        [InlineData("part1:part2")] // Trop peu de parties
        [InlineData("part1:part2:part3:part4:part5")] // Trop de parties
        public void VerifyPassword_InvalidHashFormat_ShouldReturnFalse(string invalidHash)
        {
            // Arrange
            var password = "TestPassword123";

            // Act
            bool result = _passwordHasher.VerifyPassword(password, invalidHash);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_EmptyPassword_ShouldVerifyAgainstEmptyPasswordHash()
        {
            // Arrange
            var emptyPassword = "";
            string hashedEmptyPassword = _passwordHasher.HashPassword(emptyPassword);

            // Act
            bool result = _passwordHasher.VerifyPassword(emptyPassword, hashedEmptyPassword);

            // Assert
            Assert.True(result);
        }
    }
}