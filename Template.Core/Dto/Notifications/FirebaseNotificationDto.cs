namespace Template.Core.Dto.Notifications;

public class FirebaseNotificationDto
{
    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public Dictionary<string, string>? Data { get; set; }
}