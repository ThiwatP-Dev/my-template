using Newtonsoft.Json;

namespace Template.Service.Dto;

public class CreateFacultyDto
{
    [JsonProperty("code")]
    public required string Code { get; set; }
    
    [JsonProperty("name")]
    public required string Name { get; set; }
    
    [JsonProperty("logo")]
    public string? Logo { get; set; }
}

public class FacultyDto : CreateFacultyDto
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
}