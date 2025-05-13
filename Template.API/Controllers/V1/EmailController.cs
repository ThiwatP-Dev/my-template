using Microsoft.AspNetCore.Mvc;
using Template.Core.Emails.Interfaces;

namespace Template.API.Controllers.V1;

[ApiController]
[Route(BaseUrl)]
public class EmailController(IEmailHelper emailHelper, 
                             ILoggerFactory loggerFactory) : BaseController
{
    private readonly IEmailHelper _emailHelper = emailHelper;
    private readonly ILogger _logger = loggerFactory.CreateLogger<EmailController>();

    [HttpGet]
    public async Task<IActionResult> SendAsync([FromQuery] string sendTo)
    {
        var body = "<h1>Welcome!</h1>";
        await _emailHelper.SendAsync(sendTo, body);

        _logger.LogInformation("Email send to {sendTo}", sendTo);
        
        return Ok();
    }
}