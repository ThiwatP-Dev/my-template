using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Template.Service.Dto.Localizations;

public class InstituteDto : BaseLocalizationDto
{
    [JsonProperty("name")]
    [FromForm(Name = "name")]
    public required string Name { get; set; }
}