using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TweetbookApi.Contracts.V1.Requests;
using TweetbookApi.Contracts.V1.Responses;
using TweetbookApi.Helpers;
using TweetbookApi.Models;
using TweetbookApi.Services;

namespace TweetbookApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public UsersController(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string filter,
            [FromQuery] List<string> orderBy,
            [FromQuery] int pageSize = Constants.DEFAULT_PAGE_SIZE,
            [FromQuery] int pageNumber = Constants.DEFAULT_PAGE_NUMBER)
        {
            var users = await _identityService.GetUsersAsync(pageSize, pageNumber, filter, orderBy, true);
            var dto = _mapper.Map<List<UserDtoResponse>>(users);

            Response.AddPagination(pageNumber, pageSize, users.TotalPages, users.TotalCount);

            return Ok(dto);
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _identityService.GetUserByIdAsync(id, true);

            if (user == null)
            {
                return NotFound();
            }

            var dto = _mapper.Map<UserDtoResponse>(user);

            return Ok(dto);
        }

        [HttpPost("{id}/changepassword")]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordRequest request)
        {
            var currentUserId = _identityService.GetCurrentUserId();

            if (currentUserId != id)
            {
                return Unauthorized();
            }

            var result = await _identityService.ChangePasswordAsync(id, request.CurrentPassword, request.NewPassword);

            if (!result.Succeded)
            {
                return BadRequest(new ApiErrorResponse(result.Errors));
            }

            return Ok(new AuthSuccessResponse(result.Token));
        }

        [HttpPost("{id}/resetpassword")]
        public async Task<IActionResult> ResetPassword(int id, ResetPasswordRequest request)
        {
            var currentUserId = _identityService.GetCurrentUserId();

            if (currentUserId != id)
            {
                return Unauthorized();
            }

            var result = await _identityService.ResetPasswordAsync(id, request.Token, request.NewPassword);

            if (!result.Succeded)
            {
                return BadRequest(new ApiErrorResponse(result.Errors));
            }

            return Ok(new AuthSuccessResponse(result.Token));
        }

        [HttpGet("{id}/resetpasswordtoken")]
        public async Task<IActionResult> GenerateResetPasswordToken(int id)
        {
            var currentUserId = _identityService.GetCurrentUserId();

            if (currentUserId != id)
            {
                return Unauthorized();
            }

            var token = await _identityService.GeneratePasswordResetTokenAsync(id);

            // TODO: Enviar token al email del usuario

            return Ok(new AuthSuccessResponse(token));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserInfoRequest request)
        {
            var currentUserId = _identityService.GetCurrentUserId();

            if (currentUserId != id || request.Id != currentUserId)
            {
                return Unauthorized();
            }

            var result = await _identityService.UpdateUserInfoAsync(request.Id, request.Username, request.Email);

            if (!result.Succeded)
            {
                return BadRequest(new ApiErrorResponse(result.Errors));
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userExists = await _identityService.UserExistsAsync(id);

            if (!userExists)
            {
                return NotFound();
            }

            await _identityService.DeleteUserAsync(id);

            return NoContent();
        }

        [HttpPut("{id}/roles")]
        public async Task<IActionResult> UpdateUserRoles(int id, UpdateUserRolesRequest request)
        {
            var userExists = await _identityService.UserExistsAsync(id);

            if (!userExists)
            {
                return NotFound();
            }

            var result = await _identityService.UpdateUserRolesAsync(id, request.RolesIds);

            if (!result.Succeded)
            {
                return BadRequest(new ApiErrorResponse(result.Errors));
            }

            return NoContent();
        }
    }
}