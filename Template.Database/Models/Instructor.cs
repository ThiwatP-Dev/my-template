using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Database.Models;

[Table("Instructors")]
public class Instructor : ApplicationUser
{
    public required string Email { get; set; }
}