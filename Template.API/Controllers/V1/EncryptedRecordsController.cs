using Microsoft.AspNetCore.Mvc;
using Template.Service.Dto;
using Template.Service.Interfaces;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class EncryptedRecordsController(IEncryptedRecordService encryptedRecordService,
                                        ILoggerFactory loggerFactory) : BaseController
{
    private readonly IEncryptedRecordService _encryptedRecordService = encryptedRecordService;
    private readonly ILogger _logger = loggerFactory.CreateLogger<EncryptedRecordsController>();

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateEncryptedRecordDto request)
    {
        var userId = GetCurrentUser();

        _logger.LogInformation("Create encrypted record.");

        await _encryptedRecordService.CreateAsync(request);

        _logger.LogInformation("Encrypted record created.");

        var location = Url.Action(nameof(CreateAsync));
        return Created(location, null);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Get all encrypted records.");

        var response = await _encryptedRecordService.GetAllAsync();

        _logger.LogInformation("Search succeeded with total items ({itemCount}).", response.Count());

        return Ok(response);
    }
}