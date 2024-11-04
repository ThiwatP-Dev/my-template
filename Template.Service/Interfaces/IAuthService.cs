using Template.Service.Dto.Authentication;

namespace Template.Service.Interfaces;

public interface IAuthService
{
    Task<AccessTokenResponseDto> LoginAsync(string username, string password);
    Task<AccessTokenResponseDto> RefreshAsync(string token);
    Task<AccessTokenResponseDto> LoginGoogleAsync(string token);
    Task LogoutAsync(string token);
    Task<bool> IsTokenExpiredAsync(string token);
}