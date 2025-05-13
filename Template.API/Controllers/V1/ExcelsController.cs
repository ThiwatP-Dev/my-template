using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Template.Service.Dto;
using Template.Utility.Providers;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class ExcelsController(ILoggerFactory loggerFactory) : BaseController
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<ExcelsController>();

    [HttpPost]
    public async Task<IActionResult> ReadAsync([FromForm] FileRequestDto request)
    {
        _logger.LogInformation("Reading excel ({fileName}).", request.File.FileName);

        var stopwatch = Stopwatch.StartNew();
        
        await ExcelProvider.ReadAsync(request.File, true);

        stopwatch.Stop();
        _logger.LogInformation("Read finished. Time spent: {elapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds);

        return Ok();
    }
}