using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Recrut.Business.Services.Interfaces;
using Recrut.Data.Repositories.Interfaces;
using Recrut.Models;
using Recrut.Shared.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Recrut.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserAuthRepository _userAuthRepository;
        private readonly JwtOptions _jwtOptions;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IUserAuthRepository userRepository,
            IOptions<JwtOptions> jwtOptions,
            IPasswordHasher passwordHasher)
        {
            _userAuthRepository = userRepository;
            _jwtOptions = jwtOptions.Value;
            _passwordHasher = passwordHasher;
        }

        public async Task<(bool Success, string Token)> AuthenticateAsync(string email, string password)
        {
            var user = await _userAuthRepository.GetUserByEmailWithRolesAsync(email);

            if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
            {
                return (false, string.Empty);
            }

            var token = GenerateJwtToken(user);
            return (true, token);
        }

        public async Task<bool> IsInRoleAsync(int userId, string roleName)
        {
            var roles = await _userAuthRepository.GetUserRoleNamesAsync(userId);
            return roles.Any(r => r == roleName);
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            return await _userAuthRepository.GetUserRoleNamesAsync(userId);
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Ajouter les rôles aux claims
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
