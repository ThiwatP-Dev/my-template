namespace Template.Database.Models;

public class Notification
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public required string Body { get; set; }

    public string? DataPayload { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<UserNotification>? Users { get; set; }
}