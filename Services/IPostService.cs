using System.Collections.Generic;
using System.Threading.Tasks;
using TweetbookApi.Models;

namespace TweetbookApi.Services
{
    public interface IPostService
    {
         Task<List<Post>> GetAll();
         
         Task<Post> GetById(string id);

         Task<Post> Add (Post post);

         Task<bool> Update (Post post);
    }
}