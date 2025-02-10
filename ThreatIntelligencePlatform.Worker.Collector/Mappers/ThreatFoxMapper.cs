using AutoMapper;
using ThreatIntelligencePlatform.SharedData.DTOs;
using ThreatIntelligencePlatform.SharedData.Enums;
using ThreatIntelligencePlatform.SharedData.Utils;
using ThreatIntelligencePlatform.Worker.Collector.DTOs;

namespace ThreatIntelligencePlatform.Worker.Collector.Mappers;

public class ThreatFoxMapper : Profile
{
    public ThreatFoxMapper()
    {
        CreateMap<ThreatFoxData, IoCDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Source, opt => opt.MapFrom(src => SourceName.ThreatFox.ToString()))
            .ForMember(dest => dest.FirstSeen, opt => opt.MapFrom(src => DateTimeParser.Parse(src.FirstSeen)))
            .ForMember(dest => dest.LastSeen, opt => opt.MapFrom(src => DateTimeParser.Parse(src.LastSeen)))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.IocType))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Ioc))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
            .ForMember(dest => dest.AdditionalData, opt => opt.MapFrom(src => CreateAdditionalData(src)));
    }
    
    private static Dictionary<string, string> CreateAdditionalData(ThreatFoxData src)
    {
        return new Dictionary<string, string>
        {
            { "threat_type", src.ThreatType },
            { "threat_type_desc", src.ThreatTypeDesc },
            { "confidence_level", src.ConfidenceLevel.ToString() },
            { "reference", src.Reference },
            { "reporter", src.Reporter }
        };
    }
}