namespace Recrut.Business.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Token)> AuthenticateAsync(string email, string password);
        Task<bool> IsInRoleAsync(int userId, string roleName);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);
    }
}
