using Microsoft.AspNetCore.Mvc;
using Template.Core.Emails.Interfaces;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class EmailController(EmailHelper emailHelper, 
                             ILoggerFactory loggerFactory) : BaseController
{
    private readonly EmailHelper _emailHelper = emailHelper;
    private readonly ILogger _logger = loggerFactory.CreateLogger<EmailController>();

    [HttpGet]
    public async Task<IActionResult> SendAsync([FromQuery] string sendTo)
    {
        var userId = GetCurrentUser();

        await _emailHelper.SendWithTemplateAsync(sendTo, "Test Subject", "template-mail.html", userId);

        _logger.LogInformation("Email send to {sendTo}", sendTo);
        
        return Ok();
    }
}