using Microsoft.EntityFrameworkCore;
using Recrut.Data.Repositories.Interfaces;
using Recrut.Models;

namespace Recrut.Data.Repositories
{
    public class UserAuthRepository : IUserAuthRepository
    {
        private readonly AppDbContext _context;

        public UserAuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailWithRolesAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdWithRolesAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<string>> GetUserRoleNamesAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return Enumerable.Empty<string>();

            return user.Roles.Select(r => r.Name);
        }
    }
}
