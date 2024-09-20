using Template.Service.Dto;

namespace Template.Service.Interfaces;

public interface IUserService
{
    Task InitSuperAdmin();
    Task CreateLearnerAsync(CreateLearnerDto request, Guid userId);
}