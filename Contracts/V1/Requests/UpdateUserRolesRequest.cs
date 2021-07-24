using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TweetbookApi.Contracts.V1.Requests
{
    public class UpdateUserRolesRequest
    {
        [Required]
        [MinLength(1)]
        public List<int> RolesIds { get; set; }
    }
}