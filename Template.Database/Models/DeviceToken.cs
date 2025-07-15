using Template.Database.Enums;

namespace Template.Database.Models;

public class DeviceToken
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }

    public required string Token { get; set; }

    public Platform Platform { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}