using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace TweetbookApi.Models
{
    public class Role : IdentityRole<int>
    {
        public enum RoleEnum
        {
            Administrator = 1,
            Tester = 2
        }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}