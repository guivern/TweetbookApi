using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TweetbookApi.Models;

namespace TweetbookApi.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _context;
        private readonly ILogger<PostService> _logger;

        public PostService(DataContext context, ILogger<PostService> logger)
        {
            _context = context;
            _logger = logger;

            if (!_context.Posts.Any())
            {
                var posts = new List<Post>();

                for (var i = 0; i < 5; i++)
                {
                    posts.Add(new Post { Name = $"Post {i + 1}" });
                }

                _context.Posts.AddRange(posts);
                _context.SaveChanges();
            }
            _logger = logger;
        }

        public async Task<List<Post>> GetAllAsync()
        {
            var posts = await _context.Posts.ToListAsync();
            return posts;
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);
            return post;
        }

        public async Task<Post> CreateAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<bool> UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            var updated = await _context.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeleteAsync(Post post)
        {
            _context.Posts.Remove(post);
            var deleted = await _context.SaveChangesAsync();
            return deleted > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Posts.AnyAsync(x => x.Id == id);
        }
    }
}