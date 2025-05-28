using Newtonsoft.Json;

namespace Template.Service.Dto;

public class ValidatedObjectDto
{
    [JsonProperty("email")]
    public required string Email { get; set; }

    [JsonProperty("phoneNumber")]
    public required string PhoneNumber { get; set; }

    [JsonProperty("password")]
    public required string Password { get; set; }
}