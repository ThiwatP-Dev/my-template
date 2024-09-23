namespace Template.Core.Configs;

public class JWTConfiguration
{
    public string ValidAudience { get; set; } = string.Empty;

    public string ValidIssuer { get; set; } = string.Empty;

    public int TokenExpiryMinute { get; set; }

    public int RefreshTokenExpiryMinute { get; set; }

    public string Secret { get; set; } = string.Empty;

    public string RefreshTokenSecret { get; set; } = string.Empty;
}