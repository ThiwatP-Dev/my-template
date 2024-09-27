using Template.Service.Dto;
using Template.Service.Dto.Authentication;

namespace Template.Service.Interfaces;

public interface IUserService
{
    Task GenerateSuperAdmin();
    Task<Guid> CreateLearnerAsync(CreateLearnerDto request, Guid userId);
}