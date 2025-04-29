using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Core.Configs;
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

        var location = Url.Action(nameof(LoginAsync), new { id = userId });
        return Created(location, response);
    }

    [AllowAnonymous]
    [HttpPost("login/google")]
    public async Task<IActionResult> LoginGoogleAsync()
    {
        _logger.LogInformation("Login with google.");

        var authHeaderValue = Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrEmpty(authHeaderValue))
        {
            _logger.LogError("Unauthorize.");

            return Unauthorized();
        }

        var response = await _authService.LoginGoogleAsync(authHeaderValue);

        _logger.LogInformation("Login succeeded.");

        var userId = GetCurrentUser();

        var location = Url.Action(nameof(LoginAsync), new { id = userId });
        return Created(location, response);
    }

    [Authorize(AuthenticationSchemes = AzureADConfiguration.AzureAD)]
    [HttpPost("login/azureAD")]
    public async Task<IActionResult> LoginAzureADAsync()
    {
        _logger.LogInformation("Login with azure ad.");

        var token = Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Unauthorize.");

            return Unauthorized();
        }

        var response = await _authService.LoginAzureADAsync(token);

        _logger.LogInformation("Login succeeded.");

        var userId = GetCurrentUser();

        var location = Url.Action(nameof(LoginAzureADAsync), new { id = userId });
        return Created(location, response);
    }

    [Authorize(AuthenticationSchemes = CustomAuthenticationSchemeConstant.ClientSecret)]
    [HttpPost("generate-super-admin")]
    public async Task<IActionResult> GenerateSuperAdminAsync()
    {
        _logger.LogInformation("Generate super admin.");

        await _userService.GenerateSuperAdmin();

        _logger.LogInformation("Super admin generated.");
        var location = Url.Action(nameof(GenerateSuperAdminAsync));
        return Created(location, null);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation("User logout.");
        var token = Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Unauthorize.");

            return Unauthorized();
        }

        await _authService.LogoutAsync(token);

        _logger.LogInformation("Logout succeeded.");
        var location = Url.Action(nameof(Logout));
        return Created(location, null);
    }

    [Authorize(AuthenticationSchemes = CustomAuthenticationSchemeConstant.ClientSecret)]
    [HttpGet("line/url")]
    public async Task<IActionResult> GetLineLoginUrlAsync([FromQuery] string phoneNumber)
    {
        _logger.LogInformation("Get line login url.");

        var response = await _authService.GetLineLoginUrlAsync(phoneNumber);

        _logger.LogInformation("Login url created.");

        return Ok(response);
    }

    [Authorize(AuthenticationSchemes = CustomAuthenticationSchemeConstant.ClientSecret)]
    [HttpPost("line")]
    public async Task<IActionResult> LoginLineAsync([FromBody] LineLoginRequestDto request)
    {
        _logger.LogInformation("Login with line.");

        var response = await _authService.LoginLineAsync(request);

        _logger.LogInformation("Login succeeded.");

        var userId = GetCurrentUser();

        var location = Url.Action(nameof(LoginLineAsync), new { id = userId });
        return Created(location, response);
    }
}