using Newtonsoft.Json;

namespace Template.Service.Dto.Authentication;

public class GoogleTokenRequestDto
{
    [JsonProperty("token")]
    public required string Token { get; set; }
}