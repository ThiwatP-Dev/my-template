using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Core.Constants;
using Template.Service.Dto.Authentication;
using Template.Service.Interfaces;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class AuthController(IAuthService authService,
                            IUserService userService) : BaseController
{
    private readonly IAuthService _authService = authService;
    private readonly IUserService _userService = userService;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] AccessTokenRequestDto request)
    {
        var response = await _authService.LoginAsync(request.Username, request.Password);

        var userId = GetCurrentUser();
        
        var location = Url.Action(nameof(LoginAsync), new { id = userId }) ?? $"/{userId}";
        return Created(location, response);
    }

    [AllowAnonymous]
    [HttpPost("login/google")]
    public async Task<IActionResult> LoginGoogleAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeaderValue))
        {
            return Unauthorized();
        }

        var response = await _authService.LoginGoogleAsync(authHeaderValue!);

        var userId = GetCurrentUser();
        
        var location = Url.Action(nameof(LoginAsync), new { id = userId }) ?? $"/{userId}";
        return Created(location, response);
    }

    [Authorize(AuthenticationSchemes = CustomAuthenticationSchemeConstant.ClientSecret)]
    [HttpPost("generate-super-admin")]
    public async Task <IActionResult> GenerateSuperAdmin()
    {
        await _userService.GenerateSuperAdmin();
        var location = Url.Action(nameof(GenerateSuperAdmin));
        return Created(location, null);
    }
}