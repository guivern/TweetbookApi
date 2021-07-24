using System;

namespace TweetbookApi.Models
{
    public class EntityBase
    {
        public long Id { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}