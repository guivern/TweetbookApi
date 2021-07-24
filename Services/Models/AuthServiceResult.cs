using System.Collections.Generic;

namespace TweetbookApi.Models
{
    public class AuthServiceResult : ServiceResult
    {
        public string Token { get; set; }

        public AuthServiceResult()
        {
            Errors = new List<string>();
        }

        public AuthServiceResult(bool succeded, List<string> errors)
        {
            this.Succeded = succeded;
            this.Errors = errors;
        }
    }
}