using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Core.Constants;
using Template.Service.Dto.Authentication;
using Template.Service.Interfaces;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class AuthController(IAuthService authService,
                            IUserService userService,
                            ILoggerFactory loggerFactory) : BaseController
{
    private readonly IAuthService _authService = authService;
    private readonly IUserService _userService = userService;
    private readonly ILogger _logger = loggerFactory.CreateLogger<AuthController>();

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] AccessTokenRequestDto request)
    {
        _logger.LogInformation("Login with username: {username}.", request.Username);

        var response = await _authService.LoginAsync(request.Username, request.Password);

        _logger.LogInformation("Login succeeded.");

        var userId = GetCurrentUser();
        
        var location = Url.Action(nameof(LoginAsync), new { id = userId }) ?? $"/{userId}";
        return Created(location, response);
    }

    [AllowAnonymous]
    [HttpPost("login/google")]
    public async Task<IActionResult> LoginGoogleAsync()
    {
        _logger.LogInformation("Login with google.");

        if (!Request.Headers.TryGetValue("Authorization", out var authHeaderValue))
        {
            _logger.LogError("Unauthorize.");

            return Unauthorized();
        }

        var response = await _authService.LoginGoogleAsync(authHeaderValue!);

        _logger.LogInformation("Login succeeded.");

        var userId = GetCurrentUser();
        
        var location = Url.Action(nameof(LoginAsync), new { id = userId }) ?? $"/{userId}";
        return Created(location, response);
    }

    [Authorize(AuthenticationSchemes = CustomAuthenticationSchemeConstant.ClientSecret)]
    [HttpPost("generate-super-admin")]
    public async Task <IActionResult> GenerateSuperAdmin()
    {
        _logger.LogInformation("Generate super admin.");

        await _userService.GenerateSuperAdmin();

        _logger.LogInformation("Super admin generated.");
        var location = Url.Action(nameof(GenerateSuperAdmin));
        return Created(location, null);
    }
}