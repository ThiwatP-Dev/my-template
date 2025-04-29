namespace Template.Core.Configs;

public class LineConfiguration
{
    public const string Line = "Line";

    public string Url { get; set; } = string.Empty;

    public string RedirectUrl { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;
}