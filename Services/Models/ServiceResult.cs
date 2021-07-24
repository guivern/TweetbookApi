using System.Collections.Generic;

namespace TweetbookApi.Models
{
    public class ServiceResult
    {
        public bool Succeded { get; set; }
        public List<string> Errors { get; set; }

        public ServiceResult()
        {
            Succeded = false;
            Errors = new List<string>();
        }
    }
}