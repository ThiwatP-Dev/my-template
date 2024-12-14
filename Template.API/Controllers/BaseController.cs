using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Database.Enums;

namespace Template.API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class BaseController : ControllerBase
{
    protected const string BaseUrl = "api/v{version:apiVersion}/[controller]";

    [ApiExplorerSettings(IgnoreApi = true)]
    public Guid GetCurrentUser()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var userId))
        {
            return Guid.Empty;
        }
    
        return userId;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public LanguageCode GetRequestedLanguage()
    {
        if (Request.Headers.TryGetValue("Accept-Language", out var language))
        {
            if (Enum.TryParse(language, out LanguageCode languageCode))
            {
                return languageCode;
            }
        }
        
        return LanguageCode.EN;
    }
}