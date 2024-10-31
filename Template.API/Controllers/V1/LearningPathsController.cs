using Microsoft.AspNetCore.Mvc;
using Template.Service.Dto;
using Template.Service.Interfaces;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class LearningPathsController(ILearningPathService learningPathService,
                                     ILoggerFactory loggerFactory) : BaseController
{
    private readonly ILearningPathService _learningPathService = learningPathService;
    private readonly ILogger _logger = loggerFactory.CreateLogger<LearningPathsController>();

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateLearningPathDto request)
    {
        var userId = GetCurrentUser();

        _logger.LogInformation("Create learning path with code {code}.", request.Name);

        var learningPathId = await _learningPathService.CreateAsync(request, userId);

        _logger.LogInformation("Learning path created with id {learningPathId}.", learningPathId);

        var location = Url.Action(nameof(CreateAsync), new { id = learningPathId });
        return Created(location, null);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Get all learning paths.");

        var response = await _learningPathService.GetAllAsync();

        _logger.LogInformation("Search succeeded with total items ({itemCount}).", response.Count());

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] CreateLearningPathDto request)
    {
        var userId = GetCurrentUser();

        _logger.LogInformation("Update learning path with id ({id}).", id);

        await _learningPathService.UpdateAsync(id, request, userId);

        _logger.LogInformation("Learning path updated.");

        return Ok();
    }
}