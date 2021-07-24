using System.ComponentModel.DataAnnotations;

namespace TweetbookApi.Contracts.V1.Requests
{
    public class UpdateUserInfoRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }
    }
}