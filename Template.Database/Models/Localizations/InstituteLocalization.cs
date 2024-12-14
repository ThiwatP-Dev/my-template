using Template.Database.Models.Partials;

namespace Template.Database.Models.Localizations;

public class InstituteLocalization : BaseLocalization
{
    public Guid InstituteId { get; set; }

    public required string Name { get; set; }
}