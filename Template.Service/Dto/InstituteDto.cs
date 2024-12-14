using Newtonsoft.Json;
using Template.Core;
using Template.Database.Enums;
using Template.Database.Models;

namespace Template.Service.Dto;

public class CreateInstituteDto
{
    [JsonProperty("localizations")]
    public IEnumerable<Localizations.InstituteDto> Localizations { get; set; } = [];
}

public class InstituteDto : CreateInstituteDto
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }
}

public static class InstituteMapper
{
    public static InstituteDto Map(Institute model, LanguageCode language)
    {
        var locale = model.GetLocale(language);
        var response = new InstituteDto
        {
            Id = model.Id,
            Name = locale.Name,
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