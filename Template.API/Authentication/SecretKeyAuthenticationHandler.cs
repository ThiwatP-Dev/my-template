using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Template.API.Authentication;

public class SecretKeyAuthenticationHandler(IOptionsMonitor<SecretKeyAuthenticationSchemeOptions> options,
                                            ILoggerFactory logger,
                                            UrlEncoder encoder) : AuthenticationHandler<SecretKeyAuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var requestedSecretKey = Context.Request.Headers["x-api-key"];
        if (requestedSecretKey != Options.SecretKey)
        {
            return await Task.FromResult(AuthenticateResult.Fail("Client Secret Unauthorized"));
        }

        var claims = new List<Claim> { };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

