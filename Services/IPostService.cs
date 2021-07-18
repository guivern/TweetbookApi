using System.Collections.Generic;
using System.Threading.Tasks;
using TweetbookApi.Models;

namespace TweetbookApi.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetAllAsync();

        Task<Post> GetByIdAsync(int id);

        Task<Post> CreateAsync(Post post);

        Task<bool> UpdateAsync(Post post);

        Task<bool> DeleteAsync(Post post);

        Task<bool> ExistsAsync(int id);
    }
}