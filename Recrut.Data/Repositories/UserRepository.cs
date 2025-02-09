using Microsoft.EntityFrameworkCore;
using Recrut.Data.Repositories.Interfaces;
using Recrut.Models;

namespace Recrut.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<int> DeleteUserByEmailAsync(string email)
        {
            return await _dbSet.Where(u => u.Email == email).ExecuteDeleteAsync();
        }
    }
}
