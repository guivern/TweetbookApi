using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TweetbookApi.Helpers;
using TweetbookApi.Models;

namespace TweetbookApi.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signIngManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityService(UserManager<User> userManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
            DataContext context, SignInManager<User> signIngManager)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _signIngManager = signIngManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCurrentUserId()
        {
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        public async Task<AuthServiceResult> RegisterUserAsync(string username, string email, string password, List<int> rolesIds)
        {
            var userRoles = new List<UserRole>();
            var userAvailabilityResult = await ValidateUserAvailabilityAsync(username, email);
            var rolesValidationResult = await ValidateRolesAsync(rolesIds);
            var newUser = new User { Email = email, UserName = username };

            if (!userAvailabilityResult.Succeded)
            {
                return new AuthServiceResult { Succeded = false, Errors = userAvailabilityResult.Errors };
            }

            var createdUser = await _userManager.CreateAsync(newUser, password);

            if (!createdUser.Succeeded)
            {
                return new AuthServiceResult { Succeded = false, Errors = createdUser.Errors.Select(x => x.Description).ToList() };
            }

            if (!rolesValidationResult.Succeded)
            {
                return new AuthServiceResult { Succeded = false, Errors = rolesValidationResult.Errors };
            }

            foreach (var roleId in rolesIds)
            {
                userRoles.Add(new UserRole { RoleId = roleId, UserId = newUser.Id });
            }

            await _context.UserRoles.AddRangeAsync(userRoles);
            await _context.SaveChangesAsync();

            return new AuthServiceResult { Succeded = true, Token = GenerateToken(newUser) };
        }

        public async Task<ServiceResult> UpdateUserInfoAsync(int userId, string username, string email)
        {
            var availabilityResult = await ValidateUserAvailabilityAsync(username, email, userId);

            if (!availabilityResult.Succeded)
            {
                return new ServiceResult { Succeded = false, Errors = availabilityResult.Errors };
            }

            var user = await GetUserByIdAsync(userId);
            user.UserName = username;
            user.Email = email;
            user.LastModified = DateTime.Now;

            var updateResult = await _userManager.UpdateAsync(user);

            return new ServiceResult
            {
                Succeded = updateResult.Succeeded,
                Errors = updateResult.Errors.Select(x => x.Description).ToList()
            };
        }

        public async Task<User> GetUserByIdAsync(int id, bool includeRoles = false)
        {
            var query = _context.Users.AsNoTracking().AsQueryable().Where(x => !x.IsDeleted);

            if (includeRoles)
            {
                query = query.Include(x => x.UserRoles).ThenInclude(x => x.Role);
            }

            return await query.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedList<User>> GetUsersAsync(int pageSize, int pageNumber, string filter, List<string> orderBy, bool includeRoles = false)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            if (includeRoles)
            {
                query = query.Include(x => x.UserRoles).ThenInclude(x => x.Role);
            }

            query = query.Where(x => !x.IsDeleted);
            query = query.Filter<User>(filter, new List<string> { nameof(User.Email), nameof(User.UserName) });
            query = query.Sort<User>(orderBy);

            return await PagedList<User>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<User> GetUserByUsernameAsync(string useraname)
        {
            return await _context.Users.AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Where(x => x.NormalizedUserName == useraname.Trim().ToUpper() || x.NormalizedEmail == useraname.Trim().ToUpper())
                .SingleOrDefaultAsync();
        }

        public async Task<AuthServiceResult> LoginAsync(string username, string password)
        {
            var result = new AuthServiceResult();
            var user = await GetUserByUsernameAsync(username);

            if (user == null)
            {
                result.Succeded = false;
                result.Errors.Add($"User {username} does not exists");
                return result;
            }

            var loginResult = await _signIngManager.CheckPasswordSignInAsync(user, password, false);
            result.Succeded = loginResult.Succeeded;
            result.Token = loginResult.Succeeded ? GenerateToken(user) : null;

            return result;
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<AuthServiceResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var result = new AuthServiceResult();
            var user = await GetUserByIdAsync(userId);
            var currentPasswordValidation = await _signIngManager.CheckPasswordSignInAsync(user, currentPassword, false);

            if (!currentPasswordValidation.Succeeded)
            {
                result.Succeded = false;
                result.Errors.Add($"Current password is not valid");
                return result;
            }

            if (currentPassword.Equals(newPassword))
            {
                result.Succeded = false;
                result.Errors.Add("The new password must be different to current password");
                return result;
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            result.Succeded = changePasswordResult.Succeeded;
            result.Token = changePasswordResult.Succeeded ? GenerateToken(user) : null;
            result.Errors = changePasswordResult.Errors.Select(x => x.Description).ToList();

            return result;
        }

        public async Task<AuthServiceResult> ResetPasswordAsync(int userId, string token, string newPassword)
        {
            var result = new AuthServiceResult();
            var user = await GetUserByIdAsync(userId);
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, newPassword);

            result.Succeded = resetPasswordResult.Succeeded;
            result.Token = resetPasswordResult.Succeeded ? GenerateToken(user) : null;
            result.Errors = resetPasswordResult.Errors.Select(x => x.Description).ToList();

            return result;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<ServiceResult> UpdateUserRolesAsync(int userId, List<int> rolesIds)
        {
            var result = new ServiceResult();
            var rolesValidationResult = await ValidateRolesAsync(rolesIds);
            var userRoles = new List<UserRole>();

            if (!rolesValidationResult.Succeded)
            {
                result.Succeded = false;
                result.Errors = rolesValidationResult.Errors;
                return result;
            }

            foreach (var roleId in rolesIds)
            {
                userRoles.Add(new UserRole { RoleId = roleId, UserId = userId });
            }

            var user = await _context.Users.Include(x => x.UserRoles).SingleOrDefaultAsync(x => x.Id == userId);
            user.UserRoles = userRoles;
            user.LastModified = DateTime.Now;

            var updateResult = await _context.SaveChangesAsync();
            result.Succeded = updateResult > 0;

            return result;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await GetUserByIdAsync(userId);
            user.IsDeleted = true;
            user.LastModified = DateTime.Now;

            _context.Update(user);

            var result = await _context.SaveChangesAsync() > 0;

            return result;
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            return await _context.Users.AnyAsync(x => !x.IsDeleted && x.Id == userId);
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Auth0:key"]);
            double expirationInHours = 1;

            double.TryParse(_configuration["Auth0:ExpirationInHours"], out expirationInHours);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var rolesIds = _context.UserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.Role.Id).ToList();
            foreach (var roleId in rolesIds)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleId.ToString()));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(expirationInHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Audience = _configuration["Auth0:Audience"],
                Issuer = _configuration["Auth0:Domain"]
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        private async Task<ServiceResult> ValidateRolesAsync(List<int> rolesIds)
        {
            var result = new ServiceResult();
            result.Succeded = true;

            foreach (var roleId in rolesIds)
            {
                var exists = await _context.Roles.AnyAsync(r => r.Id == roleId);

                if (!exists)
                {
                    result.Succeded = false;
                    result.Errors.Add($"Role with Id {roleId} does not exist");
                }
            }

            return result;
        }

        private async Task<ServiceResult> ValidateUserAvailabilityAsync(string username, string email, int? userIdEditing = null)
        {
            var result = new ServiceResult();
            result.Succeded = true;

            var emailIsAvailable = !await _context.Users
                .Where(x => !x.IsDeleted)
                .AnyAsync(x => x.NormalizedEmail == email.Trim().ToUpper() && x.Id != userIdEditing);

            var usernameIsAvailable = !await _context.Users
                .Where(x => !x.IsDeleted)
                .AnyAsync(x => x.NormalizedUserName == username.Trim().ToUpper() && x.Id != userIdEditing);

            if (!emailIsAvailable)
            {
                result.Succeded = false;
                result.Errors.Add("User with this email already exists");
            }

            if (!usernameIsAvailable)
            {
                result.Succeded = false;
                result.Errors.Add("User with this username already exists");
            }

            return result;
        }
    }
}