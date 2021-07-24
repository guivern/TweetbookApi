using AutoMapper;
using TweetbookApi.Models;
using TweetbookApi.Contracts.V1.Responses;

namespace TweetbookApi.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDtoResponse>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles));
            CreateMap<Role, RoleDtoResponse>();
            CreateMap<UserRole, RoleDtoResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Role.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Role.Name));
        }
    }
}