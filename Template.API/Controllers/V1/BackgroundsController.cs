using Microsoft.AspNetCore.Mvc;
using Template.Database;
using Template.Service.src;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class BackgroundsController(IServiceScopeFactory scopeFactory) : BaseController
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    [HttpPost("run")]
    public IActionResult Run()
    {
        Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();

            var worker = scope.ServiceProvider.GetRequiredService<BackgroundJobService>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<BackgroundsController>>();

            logger.LogInformation("Background task started.");

            var id = await worker.RunBackgroundJobAsync();

            await worker.CompleteJobAsync(id);

            logger.LogInformation("Done: {id}", id);
        });

        return Ok();
    }
}