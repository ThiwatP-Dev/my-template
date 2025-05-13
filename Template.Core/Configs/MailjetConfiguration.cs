namespace Template.Core.Configs;

public class MailjetConfiguration
{
    public const string Mailjet = "Mailjet";

    public string ApiKey { get; set; } = string.Empty;

    public string ApiSecret { get; set; } = string.Empty;

    public string SenderEmail { get; set; } = string.Empty;

    public string SenderName { get; set; } = string.Empty;
}