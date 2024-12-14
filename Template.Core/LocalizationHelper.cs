using Template.Database.Enums;
using Template.Database.Models.Partials;

namespace Template.Core;

public static class LocalizationHelper
{
    public static TEntity GetLocale<TEntity>(this LocalizableEntity<TEntity> entity, LanguageCode language)
        where TEntity : BaseLocalization
    {
        var response = entity.Localizations.SingleOrDefault(x => x.Language == language);
        if (response is null)
        {
            return entity.GetDefault();
        }

        return response;
    }
}