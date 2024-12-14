namespace Template.Database.Models.Partials;

public abstract class LocalizableEntity<T>
{
    public virtual ICollection<T> Localizations { get; set; } = [];

    public abstract T GetDefault();
}