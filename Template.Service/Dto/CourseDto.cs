using Newtonsoft.Json;
using Template.Database.Models;

namespace Template.Service.Dto;

public class CreateCourseDto
{
    [JsonProperty("code")]
    public required string Code { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("lecturers")]
    public IEnumerable<Guid>? Lecturers { get; set; }
}

public class CourseDto : CreateCourseDto
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
}

public static class CourseMapper
{
    public static CourseDto Map(Course model)
    {
        var response = new CourseDto
        {
            Id = model.Id,
            Code = model.Code,
            Name = model.Name,
            Lecturers = model.Lecturers is null 
                        || !model.Lecturers.Any() ? null
                                                  : model.Lecturers.Select(x => x.Id)
        };

        return response;
    }
}