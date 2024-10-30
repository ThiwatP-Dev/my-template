using Template.Service.Dto;

namespace Template.Service.Interfaces;

public interface IUserService
{
    Task GenerateSuperAdmin();
    Task<Guid> CreateLearnerAsync(CreateLearnerDto request, Guid userId);
    Task<Guid> CreateLecturerAsync(CreateLecturerDto request, Guid userId);
}