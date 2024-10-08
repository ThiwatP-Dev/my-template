using Template.Database.Enums;

namespace Template.Database.Models;

public class Learner() : ApplicationUser(Role.LEARNER)
{
    public required string Code { get; set; }

    public required string Email { get; set; }

    public required string PhoneNumber { get; set; }

    public string? ProfileUrl { get; set; }
}