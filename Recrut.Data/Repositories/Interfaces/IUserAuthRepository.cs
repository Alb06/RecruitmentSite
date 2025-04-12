using Recrut.Models;

namespace Recrut.Data.Repositories.Interfaces
{
    public interface IUserAuthRepository
    {
        Task<User?> GetUserByEmailWithRolesAsync(string email);
        Task<User?> GetUserByIdWithRolesAsync(int userId);
        Task<IEnumerable<string>> GetUserRoleNamesAsync(int userId);
    }
}
