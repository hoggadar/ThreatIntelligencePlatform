using AutoMapper;
using ThreatIntelligencePlatform.Shared.DTOs;
using ThreatIntelligencePlatform.Shared.Enums;
using ThreatIntelligencePlatform.Shared.Utils;
using ThreatIntelligencePlatform.SharedData.DTOs.TweetFeed;

namespace ThreatIntelligencePlatform.Worker.IoCCollector.Mappers;

public class TweetFeedMapper : Profile
{
    public TweetFeedMapper()
    {
        CreateMap<TweetFeedResponseDto, IoCDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (string?)null))
            .ForMember(dest => dest.Source, opt => opt.MapFrom(src => SourceName.TweetFeed.ToString()))
            .ForMember(dest => dest.FirstSeen, opt => opt.MapFrom(src => DateTimeParser.Parse(src.Date)))
            .ForMember(dest => dest.LastSeen, opt => opt.MapFrom(src => (DateTime?)null))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
            .ForMember(dest => dest.AdditionalData, opt => opt.MapFrom(src => CreateAdditionalData(src)));
    }
    
    private Dictionary<string, string> CreateAdditionalData(TweetFeedResponseDto src)
    {
        return new Dictionary<string, string>
        {
            { "user", src.User },
            { "tweet", src.Tweet }
        };
    }
}