using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Database.Models;

[Table("ApplicationUsers")]
public class ApplicationUser
{
    public Guid Id { get; set; }

    public required string Username { get; set; }

    public required string HashedPassword { get; set; }

    public required string HashedKey { get; set; }

    public required string FirstName { get; set; }

    public string? LastName { get; set; }
}