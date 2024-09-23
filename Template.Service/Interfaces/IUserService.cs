using Template.Service.Dto;

namespace Template.Service.Interfaces;

public interface IUserService
{
    Task InitSuperAdmin();
    Task CreateLearnerAsync(CreateLearnerDto request, Guid userId);
    Task<AccessTokenResponseDto> Login(string username, string password);
    Task<AccessTokenResponseDto> Refresh(string token);
}