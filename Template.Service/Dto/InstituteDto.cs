namespace Template.Service.Dto;

public class CreateInstituteDto
{
    public required string Name { get; set; }
}

public class InstituteDto : CreateInstituteDto
{
    public Guid Id { get; set; }
}