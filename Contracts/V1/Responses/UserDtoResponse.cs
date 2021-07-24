using System.Collections.Generic;
using TweetbookApi.Models;

namespace TweetbookApi.Contracts.V1.Responses
{
    public class UserDtoResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<RoleDtoResponse> Roles { get; set; }
    }
}