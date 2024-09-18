using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Database.Models;

[Table("Learners")]
public class Learner : ApplicationUser
{
    public required string Code { get; set; }

    public required string Email { get; set; }
}