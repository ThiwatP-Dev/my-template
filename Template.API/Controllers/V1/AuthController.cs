using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Core.Constants;
using Template.Service.Dto;
using Template.Service.Interfaces;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class AuthController(IUserService userService) : BaseController
{
    private readonly IUserService _userService = userService;

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] AccessTokenRequestDto request)
    {
        var response = await _userService.LoginAsync(request.Username, request.Password);

        var userId = GetCurrentUser();
        
        var location = Url.Action(nameof(LoginAsync), new { id = userId }) ?? $"/{userId}";
        return Created(location, response);
    }

    [Authorize(AuthenticationSchemes = CustomAuthenticationSchemeConstant.ClientSecret)]
    [HttpPost("generate-super-admin")]
    public async Task <IActionResult> InitSuperAdmin()
    {
        await _userService.GenerateSuperAdmin();
        var location = Url.Action(nameof(InitSuperAdmin));
        return Created(location, null);
    }
}