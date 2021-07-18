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
                    posts.Add(new Post { Id = $"{Guid.NewGuid()}", Name = $"Post {i}" });
                }

                _context.Posts.AddRange(posts);
                _context.SaveChanges();
            }
            _logger = logger;
        }

        public async Task<List<Post>> GetAll()
        {
            var posts = await _context.Posts.AsNoTracking().ToListAsync();
            return posts;
        }

        public async Task<Post> GetById(string id)
        {
            var post = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return post;
        }

        public async Task<Post> Add(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<bool> Update(Post post)
        {
            try
            {
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }
        }
    }
}