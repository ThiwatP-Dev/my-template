using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Template.Core;
using Template.Core.Storages.Interfaces;
using Template.Database.Enums;
using Template.Database.Models;

namespace Template.Service.Dto;

public class CreateInstituteDto
{
    [FromForm(Name = "file")]
    public IFormFile? File { get; set; }
    
    [FromForm(Name = "localizations")]
    public required IEnumerable<Localizations.InstituteDto> Localizations { get; set; }
}

public class InstituteDto
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("file")]
    public ResourceDto? File { get; set; }

    [JsonProperty("localizations")]
    public IEnumerable<Localizations.InstituteDto> Localizations { get; set; } = [];
}

public static class InstituteMapper
{
    public static async Task<InstituteDto> MapAsync(Institute model,
        LanguageCode language, IStorageHelper helper)
    {
        var locale = model.GetLocale(language);
        var response = new InstituteDto
        {
            Id = model.Id,
            Name = locale.Name,
            File = await ResourceMapper.MapAsync(model.Resource, helper),
            Localizations = (from localization in model.Localizations
                             select new Localizations.InstituteDto
                             {
                                 Language = localization.Language,
                                 Name = localization.Name
                             })
                            .ToList()
        };

        return response;
    }
}