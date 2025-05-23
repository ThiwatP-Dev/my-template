using Template.Database.Enums;

namespace Template.Database.Models;

public class Lecturer() : ApplicationUser(Role.LECTURER)
{
    public required string Email { get; set; }

    public virtual IEnumerable<Course>? Courses { get; set; }
}