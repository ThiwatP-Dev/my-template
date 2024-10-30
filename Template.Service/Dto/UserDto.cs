using Newtonsoft.Json;

namespace Template.Service.Dto;

public class CreateUserDto
{
    [JsonProperty("username")]
    public required string Username { get; set; }

    [JsonProperty("password")]
    public required string Password { get; set; }

    [JsonProperty("firstName")]
    public required string FirstName { get; set; }

    [JsonProperty("middleName")]
    public string? MiddleName { get; set; }

    [JsonProperty("lastName")]
    public string? LastName { get; set; }
}

public class CreateLearnerDto : CreateUserDto
{
    [JsonProperty("email")]
    public required string Email { get; set; }

    [JsonProperty("phoneNumber")]
    public required string PhoneNumber { get; set; }

    [JsonProperty("profileUrl")]
    public string? ProfileUrl { get; set; }
}

public class CreateLecturerDto : CreateUserDto
{
    [JsonProperty("email")]
    public required string Email { get; set; }
}