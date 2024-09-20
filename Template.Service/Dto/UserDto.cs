namespace Template.Service.Dto;

public class CreateUserDto
{
    public required string Username { get; set; }

    public required string HashedPassword { get; set; }

    public required string HashedKey { get; set; }

    public required string FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }
}

public class CreateLearnerDto : CreateUserDto
{
    public required string Email { get; set; }

    public required string PhoneNumber { get; set; }

    public string? ProfileUrl { get; set; }
}