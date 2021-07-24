using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TweetbookApi.Contracts.V1.Requests;
using TweetbookApi.Contracts.V1.Responses;
using TweetbookApi.Services;

namespace TweetbookApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationRequest request)
        {
            var errorResponse = new ApiErrorResponse();
            var successResponse = new AuthSuccessResponse();

            var result = await _identityService.RegisterUserAsync(request.Username, request.Email, request.Password, request.RolesIds);

            if (!result.Succeded)
            {
                errorResponse.Errors.AddRange(result.Errors);
                return BadRequest(errorResponse);
            }

            successResponse.Token = result.Token;
            var createdUser = await _identityService.GetUserByUsernameAsync(request.Username);

            return CreatedAtRoute("GetUserById", new { Id = createdUser.Id }, successResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var successResponse = new AuthSuccessResponse();
            var loginResult = await _identityService.LoginAsync(request.Username, request.Password);

            if (!loginResult.Succeded)
                return Unauthorized();

            successResponse.Token = loginResult.Token;

            return Ok(successResponse);
        }
    }
}