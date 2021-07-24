using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace TweetbookApi.Models
{
    public class User : IdentityUser<int>
    {
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? LastModified { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}