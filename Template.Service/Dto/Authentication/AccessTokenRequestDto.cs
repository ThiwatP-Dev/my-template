using Newtonsoft.Json;

namespace Template.Service.Dto.Authentication;

public class AccessTokenRequestDto
{
    [JsonProperty("username")]
    public required string Username { get; set; }

    [JsonProperty("password")]
    public required string Password { get; set; }
}