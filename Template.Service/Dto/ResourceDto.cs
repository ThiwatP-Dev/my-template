using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Template.Core.Storages.Interfaces;
using Template.Database.Enums;
using Template.Database.Models;

namespace Template.Service.Dto;

public class ResourceDto
{
    [JsonProperty("url")]
    public required string Url { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("date")]
    public DateTime Date { get; set; }

    [JsonProperty("type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public FileType Type { get; set; }

    [JsonProperty("size")]
    public long Size { get; set; }

    [JsonProperty("duration")]
    public double? Duration { get; set; }
}

public static class ResourceMapper
{
    public static async Task<ResourceDto?> MapAsync(Resource? model, IStorageHelper helper)
    {
        if (model is null)
        {
            return null;
        }

        var response = new ResourceDto
        {
            Url = await helper.GetUrlAsync(model.Path) ?? string.Empty,
            Name = model.Name,
            Date = model.Date,
            Type = model.Type,
            Size = model.Size,
            Duration = model.Duration
        };

        return response;
    }
}