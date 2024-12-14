using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Template.Database.Enums;

namespace Template.Service.Dto.Localizations;

public class BaseLocalizationDto
{
    [JsonProperty("language")]
    [FromForm(Name = "language")]
    [JsonConverter(typeof(StringEnumConverter))]
    public LanguageCode Language { get; set; }
}