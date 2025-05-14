using Template.Database.Enums;

namespace Template.Database.Models;

public class EmailLog
{
    public long Id { get; set; }
    
    public required string ToEmail { get; set; } = null!;

    public string? FromEmail { get; set; }

    public required string Subject { get; set; }
    
    public string? Body { get; set; }

    public EmailLogStatus Status { get; set; }

    public string? ProviderMessageId { get; set; }

    public string? ErrorMessage { get; set; }

    public string? Attachments { get; set; }

    public DateTime? SentAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}