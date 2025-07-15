using System.ComponentModel.DataAnnotations.Schema;
using Template.Database.Enums;

namespace Template.Database.Models;

public class UserNotification
{
    public Guid Id { get; set; }

    public Guid NotificationId { get; set; }

    public Guid TargetUserId { get; set; }

    public bool IsRead { get; set; }

    public NotificationSentStatus SentStatus { get; set; }

    public DateTime? SentAt { get; set; }

    public string? ErrorMessage { get; set; }

    [ForeignKey(nameof(NotificationId))]
    public Notification Notification { get; set; } = null!;
}