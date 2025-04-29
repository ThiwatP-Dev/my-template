using Newtonsoft.Json;

namespace Template.Service.Dto.Authentication;

public class LineLoginRequestDto
{
    [JsonProperty("userId")]
    public required Guid UserId { get; set; }

    [JsonProperty("authCode")]
    public required string AuthCode { get; set; }
}