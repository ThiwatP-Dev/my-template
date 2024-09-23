using Template.Service.Dto;
using Template.Service.Dto.Authentication;

namespace Template.Service.Interfaces;

public interface IUserService
{
    Task GenerateSuperAdmin();
    Task CreateLearnerAsync(CreateLearnerDto request, Guid userId);
    Task<AccessTokenResponseDto> LoginAsync(string username, string password);
    Task<AccessTokenResponseDto> RefreshAsync(string token);
}