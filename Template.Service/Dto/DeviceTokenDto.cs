using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Template.Database.Enums;

namespace Template.Service.Dto;

public class CreateDeviceTokenDto
{
    [JsonProperty("token")]
    public required string Token { get; set; }

    [JsonProperty("platform")]
    [JsonConverter(typeof(StringEnumConverter))]
    public Platform Platform { get; set; }
}