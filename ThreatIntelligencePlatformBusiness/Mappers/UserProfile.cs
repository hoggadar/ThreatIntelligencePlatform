using AutoMapper;
using ThreatIntelligencePlatform.Business.DTOs.User;
using ThreatIntelligencePlatform.DataAccess.Entities;

namespace ThreatIntelligencePlatform.Business.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, UserDto>();
        CreateMap<UserDto, UserEntity>();
        CreateMap<CreateUserDto, UserEntity>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
    }
}