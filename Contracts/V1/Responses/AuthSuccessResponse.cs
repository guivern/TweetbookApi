using TweetbookApi.Models;

namespace TweetbookApi.Contracts.V1.Responses
{
    public class AuthSuccessResponse
    {
        public AuthSuccessResponse()
        {
        }

        public AuthSuccessResponse(string token)
        {
            Token = token;
        }

        public string Token { get; set; }
    }
}