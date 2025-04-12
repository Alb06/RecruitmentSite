using AutoMapper;
using Recrut.API.DTOs;
using Recrut.Models;

namespace Recrut.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User -> UserResponseDto
            CreateMap<User, UserResponseDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => r.Name)));

            // UserCreateDto -> User
            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            // UserUpdateDto -> User
            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Condition(src => !string.IsNullOrEmpty(src.PasswordHash)))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());
        }
    }
}
