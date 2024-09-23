using Microsoft.AspNetCore.Authentication;

namespace Template.API.Authentication;

public class SecretKeyAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public string? SecretKey { get; set; }
}

