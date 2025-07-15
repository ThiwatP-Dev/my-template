using Microsoft.AspNetCore.Mvc;
using Template.Service.Dto;
using Template.Service.Interfaces;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class NotificationsController(INotificationService notificationService,
                                     ILoggerFactory loggerFactory) : BaseController
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILogger _logger = loggerFactory.CreateLogger<NotificationsController>();

    [HttpPost("token")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateDeviceTokenDto request)
    {
        var userId = GetCurrentUser();

        _logger.LogInformation("Update user's device token.");

        await _notificationService.UpdateDeviceTokenAsync(request, userId);

        _logger.LogInformation("Device token updated.");

        return Ok();
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendAsync()
    {
        _logger.LogInformation("Send notification.");

        await _notificationService.SendAsync();

        _logger.LogInformation("Notification send.");

        return Ok();
    }
}