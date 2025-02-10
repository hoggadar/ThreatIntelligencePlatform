using AutoMapper;
using ThreatIntelligencePlatform.SharedData.DTOs;
using ThreatIntelligencePlatform.SharedData.DTOs.TweetFeed;
using ThreatIntelligencePlatform.SharedData.Enums;
using ThreatIntelligencePlatform.SharedData.Utils;

namespace ThreatIntelligencePlatform.Worker.Collector.Mappers;

public class TweetFeedMapper : Profile
{
    public TweetFeedMapper()
    {
        CreateMap<TweetFeedResponse, IoCDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (string?)null))
            .ForMember(dest => dest.Source, opt => opt.MapFrom(src => SourceName.TweetFeed.ToString()))
            .ForMember(dest => dest.FirstSeen, opt => opt.MapFrom(src => DateTimeParser.Parse(src.Date)))
            .ForMember(dest => dest.LastSeen, opt => opt.MapFrom(src => (DateTime?)null))
            .ForMember(dest => dest.AdditionalData, opt => opt.MapFrom(src => CreateAdditionalData(src)));
    }
    
    private Dictionary<string, string> CreateAdditionalData(TweetFeedResponse src)
    {
        return new Dictionary<string, string>
        {
            { "user", src.User },
            { "tweet", src.Tweet }
        };
    }
}