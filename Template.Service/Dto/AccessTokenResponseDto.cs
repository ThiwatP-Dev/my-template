using Newtonsoft.Json;

namespace Template.Service.Dto;

public class AccessTokenResponseDto
{
    [JsonProperty("type")]
    public string Type { get; set; } = "Bearer";

    [JsonProperty("accessToken")]
    public required string AccessToken { get; set; }

    [JsonProperty("refreshToken")]
    public required string RefreshToken { get; set; }
    
    [JsonProperty("expiredIn")]
    public int ExpiredIn { get; set; }
}