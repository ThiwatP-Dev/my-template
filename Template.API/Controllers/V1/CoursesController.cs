using Microsoft.AspNetCore.Mvc;
using Template.Service.Dto;
using Template.Service.Interfaces;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class CoursesController(ICourseService instituteService,
                               ILoggerFactory loggerFactory) : BaseController
{
    private readonly ICourseService _courseService = instituteService;
    private readonly ILogger _logger = loggerFactory.CreateLogger<CoursesController>();

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateCourseDto request)
    {
        var userId = GetCurrentUser();

        _logger.LogInformation("Create course with code {code}.", request.Code);

        var courseId = await _courseService.CreateAsync(request, userId);

        _logger.LogInformation("Course created with id {courseId}.", courseId);

        var location = Url.Action(nameof(CreateAsync), new { id = courseId });
        return Created(location, null);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Get all courses.");

        var response = await _courseService.GetAllAsync();

        _logger.LogInformation("Search succeeded with total items ({itemCount}).", response.Count());

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] CreateCourseDto request)
    {
        var userId = GetCurrentUser();

        _logger.LogInformation("Update course with id ({id}).", id);

        await _courseService.UpdateAsync(id, request, userId);

        _logger.LogInformation("Course updated.");

        return Ok();
    }
}