using Newtonsoft.Json;
using Template.Database.Models;

namespace Template.Service.Dto;

public class CreateInstituteDto
{
    [JsonProperty("name")]
    public required string Name { get; set; }
}

public class InstituteDto : CreateInstituteDto
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
}

public static class InstituteMapper
{
    public static InstituteDto Map(Institute model)
    {
        var response = new InstituteDto
        {
            Id = model.Id,
            Name = model.Name
        };

        return response;
    }
}