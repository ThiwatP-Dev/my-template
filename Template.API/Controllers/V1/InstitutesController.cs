using Microsoft.AspNetCore.Mvc;
using Template.Database.Enums;
using Template.Service.Dto;
using Template.Service.Interfaces;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class InstitutesController(IInstituteService instituteService,
                                  ILoggerFactory loggerFactory) : BaseController
{
    private readonly IInstituteService _instituteService = instituteService;
    private readonly ILogger _logger = loggerFactory.CreateLogger<InstitutesController>();

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateInstituteDto request)
    {
        var userId = GetCurrentUser();

        _logger.LogInformation("Create institute with name {name}.", request.Localizations.SingleOrDefault(x => x.Language == LanguageCode.EN)?.Name ?? string.Empty);

        var instituteId = await _instituteService.CreateAsync(request, userId);

        _logger.LogInformation("Institute created with id {instituteId}.", instituteId);

        var location = Url.Action(nameof(CreateAsync), new { id = instituteId });
        return Created(location, null);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 25)
    {
        var language = GetRequestedLanguage();

        _logger.LogInformation("Search institute.");

        var response = await _instituteService.SearchAsync(page, pageSize, language);

        _logger.LogInformation("Search succeeded.");

        if (!response.Items.Any())
        {
            return NoContent();
        }

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var language = GetRequestedLanguage();
        
        _logger.LogInformation("Get all institute.");

        var response = await _instituteService.GetAllAsync(language);

        _logger.LogInformation("Search succeeded with total items ({itemCount}).", response.Count());

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var language = GetRequestedLanguage();

        _logger.LogInformation("Get institute by id ({id}).", id);

        var respnse = await _instituteService.GetByIdAsync(id, language);

        _logger.LogInformation("Institute found.");

        return Ok(respnse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] CreateInstituteDto request)
    {
        var userId = GetCurrentUser();

        _logger.LogInformation("Update institute with id ({id}).", id);

        await _instituteService.UpdateAsync(id, request, userId);

        _logger.LogInformation("Institute updated.");

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        _logger.LogInformation("Delete institute with id ({id}).", id);

        await _instituteService.DeleteAsync(id);

        _logger.LogInformation("Institute deleted.");

        return Ok();
    }
}