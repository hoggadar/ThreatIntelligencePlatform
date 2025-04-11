using AutoMapper;
using ThreatIntelligencePlatform.Business.DTOs.Role;
using ThreatIntelligencePlatform.Business.Entities;

namespace ThreatIntelligencePlatform.Business.Mappers;

public class RoleMapper : Profile
{
    public RoleMapper()
    {
        CreateMap<RoleEntity, RoleDto>();
    }
}