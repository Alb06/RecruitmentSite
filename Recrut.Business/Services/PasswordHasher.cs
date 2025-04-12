using Recrut.Business.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Recrut.Business.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32; // 256 bits
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;
        private const char Delimiter = ':';

        public string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                Iterations,
                Algorithm,
                KeySize);

            return string.Join(
                Delimiter,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash),
                Iterations,
                Algorithm
            );
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var parts = hashedPassword.Split(Delimiter);
            if (parts.Length != 4)
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);
            var iterations = int.Parse(parts[2]);
            var algorithm = new HashAlgorithmName(parts[3]);

            var inputHash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                algorithm,
                hash.Length);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
    }
}
