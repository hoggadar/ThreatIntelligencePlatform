using AutoMapper;
using ThreatIntelligencePlatform.Shared.DTOs;
using ThreatIntelligencePlatform.Shared.Enums;
using ThreatIntelligencePlatform.Worker.IoCCollector.DTOs;

namespace ThreatIntelligencePlatform.Worker.IoCCollector.Mappers;

public class FeodoTrackerMapper : Profile
{
    public FeodoTrackerMapper()
    {
        CreateMap<FeodoTrackerResponseDto, IoCDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (string?)null))
            .ForMember(dest => dest.Source, opt => opt.MapFrom(src => SourceName.FeodoTracker.ToString()))
            .ForMember(dest => dest.FirstSeen, opt => opt.MapFrom(src => (DateTime?)null))
            .ForMember(dest => dest.LastSeen, opt => opt.MapFrom(src => (DateTime?)null))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "ip"))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.IoC))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => new List<string>()))
            .ForMember(dest => dest.AdditionalData, opt => opt.MapFrom(src => new Dictionary<string, string>()));
    }
}