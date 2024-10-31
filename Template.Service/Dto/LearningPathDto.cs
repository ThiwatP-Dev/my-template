using Newtonsoft.Json;
using Template.Database.Models;

namespace Template.Service.Dto;

public class CreateLearningPathDto
{
    [JsonProperty("name")]
    public required string Name { get; set; }
    
    [JsonProperty("description")]
    public string? Description { get; set; }
    
    [JsonProperty("remark")]
    public string? Remark { get; set; }
}

public class LearningPathDto : CreateLearningPathDto
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
}

public static class LearningPathMapper
{
    public static LearningPathDto Map(LearningPath model)
    {
        var response = new LearningPathDto
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Properties?.Description,
            Remark = model.Properties?.Remark
        };

        return response;
    }
}