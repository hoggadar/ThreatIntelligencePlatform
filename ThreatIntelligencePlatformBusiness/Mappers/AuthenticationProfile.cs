using AutoMapper;
using ThreatIntelligencePlatform.Business.DTOs.Authentication;
using ThreatIntelligencePlatform.Business.DTOs.User;

namespace ThreatIntelligencePlatform.Business.Mappers;

public class AuthenticationProfile : Profile
{
    public AuthenticationProfile()
    {
        CreateMap<SignupDto, CreateUserDto>();
    }
}