using Newtonsoft.Json;
using ThreatIntelligencePlatform.Shared.DTOs;

namespace ThreatIntelligencePlatform.Shared.Utils;

public class IoCFormatter
{
    public static string Format(IoCDto ioc)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "yyyy-MM-dd HH:mm:ss"
        };

        return JsonConvert.SerializeObject(new
        {
            ioc.Id,
            ioc.Source,
            ioc.FirstSeen,
            ioc.LastSeen,
            ioc.Type,
            ioc.Value,
            ioc.Tags,
            AdditionalData = ioc.AdditionalData.Count > 0 ? ioc.AdditionalData : null
        }, settings);
    }
}