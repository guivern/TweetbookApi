using Microsoft.AspNetCore.Mvc;

namespace TweetbookApi.Controllers.V2
{
    [ApiController]
    [Route("api/v2/[controller]")]
    public class TestController: ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new {Text = "Hello world V2"});
        }
    }
}