using Microsoft.AspNetCore.Mvc;
using Template.Database.Enums;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class EnumController(ILoggerFactory loggerFactory) : BaseController
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<EnumController>();

    [HttpGet("platforms")]
    public IActionResult GetFacilityTypes()
    {
        _logger.LogInformation("Get platform enum.");

        var response = Enum.GetNames(typeof(Platform));

        _logger.LogInformation("Successfully get platform enum.");

        return Ok(response);
    }
}