using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TweetbookApi.Contracts.V1.Responses;
using TweetbookApi.Services;

namespace TweetbookApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RolesController: ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public RolesController(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _identityService.GetRolesAsync();
            var dto = _mapper.Map<List<RoleDtoResponse>>(roles);

            return Ok(dto);
        }
    }
}