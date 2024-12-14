using Newtonsoft.Json;

namespace Template.Service.Dto.Localizations;

public class InstituteDto : BaseLocalizationDto
{
    [JsonProperty("name")]
    public required string Name { get; set; }
}