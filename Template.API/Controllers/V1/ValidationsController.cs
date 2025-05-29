using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Template.Service.Dto;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class ValidationsController(ILoggerFactory loggerFactory) : BaseController
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<ValidationsController>();

    [HttpPost]
    public async Task<IActionResult> ValidateAsync([FromBody] ValidatedObjectDto request,
        [FromServices] IValidator<ValidatedObjectDto> validator)
    {
        _logger.LogInformation("Begin validate object.");

        await validator.ValidateAndThrowAsync(request);

        _logger.LogInformation("Request validated.");

        return Ok();
    }
}