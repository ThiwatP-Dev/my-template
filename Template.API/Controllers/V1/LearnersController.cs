using Microsoft.AspNetCore.Mvc;
using Template.Service.Dto;
using Template.Service.Interfaces;
using Template.Service.src;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class LearnersController(IUserService userService,
                                ILoggerFactory loggerFactory) : BaseController
{
    private readonly IUserService _userService = userService;
    private readonly ILogger _logger = loggerFactory.CreateLogger<LearnersController>();

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateLearnerDto request)
    {
        var userId = GetCurrentUser();

        _logger.LogInformation("Create learner with username {username}", request.Username);

        var learnerId = await _userService.CreateLearnerAsync(request, userId);

        _logger.LogInformation("Learner created with id {learnerId}", learnerId);
        
        var location = Url.Action(nameof(CreateAsync), new { id = learnerId }) ?? $"/{learnerId}";
        return Created(location, null);
    }
}