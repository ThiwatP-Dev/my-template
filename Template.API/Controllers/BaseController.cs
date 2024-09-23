using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}