using Template.Database.Models.Localizations;
using Template.Database.Models.Partials;

namespace Template.Database.Models;

public class Institute : LocalizableEntity<InstituteLocalization>
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public Resource? Resource { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public override InstituteLocalization GetDefault()
    {
        return new InstituteLocalization
        {
            Name = Name
        };
    }
}