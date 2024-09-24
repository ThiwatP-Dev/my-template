namespace Template.Core.Configs;

public class GoogleClientConfiguration
{
    public const string Client = "GoogleClient";

    public IEnumerable<string> Audience { get; set; } = [];
}