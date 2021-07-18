using Microsoft.EntityFrameworkCore;

namespace TweetbookApi.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DbSet<Post> Posts { get; set; }
    }
}