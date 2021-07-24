using System.Collections.Generic;
using System.Threading.Tasks;
using TweetbookApi.Helpers;
using TweetbookApi.Models;

namespace TweetbookApi.Services
{
    public interface IIdentityService
    {
        int GetCurrentUserId();
        Task<AuthServiceResult> RegisterUserAsync(string username, string email, string password, List<int> rolesIds);
        Task<ServiceResult> UpdateUserInfoAsync(int userId, string username, string email); 
        Task<PagedList<User>> GetUsersAsync(int pageSize, int pageNumber, string filter, List<string> orderBy, bool includeRoles = false);
        Task<User> GetUserByIdAsync(int id, bool includeRoles = false);
        Task<User> GetUserByUsernameAsync(string useraname);
        Task<List<Role>> GetRolesAsync();
        Task<AuthServiceResult> LoginAsync(string username, string password);
        Task<AuthServiceResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<AuthServiceResult> ResetPasswordAsync(int userId, string token, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(int userId);
        Task<bool> DeleteUserAsync(int userId);
        Task<ServiceResult> UpdateUserRolesAsync(int userId, List<int> rolesIds);
        Task<bool> UserExistsAsync(int userId);
    }
}