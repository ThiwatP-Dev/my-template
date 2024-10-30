using Microsoft.AspNetCore.Mvc;
using Template.Service.Dto;
using Template.Service.Interfaces;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class LecturersController(IUserService userService,
                                ILoggerFactory loggerFactory) : BaseController
{
    private readonly IUserService _userService = userService;
    private readonly ILogger _logger = loggerFactory.CreateLogger<LecturersController>();

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateLecturerDto request)
    {
        var userId = GetCurrentUser();

        _logger.LogInformation("Create lecturer with username {username}.", request.Username);

        var lecturerId = await _userService.CreateLecturerAsync(request, userId);

        _logger.LogInformation("Lecturer created with id {lecturerId}.", lecturerId);

        var location = Url.Action(nameof(CreateAsync), new { id = lecturerId });
        return Created(location, null);
    }
}