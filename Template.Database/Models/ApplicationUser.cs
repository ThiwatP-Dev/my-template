using System.ComponentModel.DataAnnotations.Schema;
using Template.Database.Enums;

namespace Template.Database.Models;

[Table("ApplicationUsers")]
public class ApplicationUser(Role role)
{
    public Guid Id { get; set; }

    public required string Username { get; set; }

    public required string HashedPassword { get; set; }

    public required string HashedKey { get; set; }

    public required string FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public Role Role { get; set; } = role;
}