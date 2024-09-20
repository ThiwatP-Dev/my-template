using System.ComponentModel.DataAnnotations.Schema;
using Template.Database.Enums;

namespace Template.Database.Models;

[Table("Lecturers")]
public class Lecturer() : ApplicationUser(Role.LECTURER)
{
    public required string Email { get; set; }
}