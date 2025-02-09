using Recrut.Models;

namespace Recrut.Data.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<int> DeleteUserByEmailAsync(string email);
    }
}
