using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Template.Core.Extensions;
using Template.Database.Models;
using Template.Service.Dto.Authentication;

namespace Template.Service.src;

public partial class UserService
{
    public async Task<AccessTokenResponseDto> LoginAsync(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException();
        }

        var user = await _userRepository.SingleOrDefaultAsync(x => x.Username.Equals(username));
        if (user is null || string.IsNullOrEmpty(user.HashedPassword))
        {
            throw new KeyNotFoundException();
        }

        var isPasswordMatched = password.IsHashHMACSHA256Match(user.HashedPassword, user.HashedKey);
        if (!isPasswordMatched)
        {
            throw new UnauthorizedAccessException();
        }

        var response = GenerateUserToken(user);

        return response;
    }

    public async Task<AccessTokenResponseDto> RefreshAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
        {
            throw new InvalidOperationException();
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.RefreshTokenSecret));
        var validations = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = authSigningKey,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var claimsPrincipal = handler.ValidateToken(token, validations, out var tokenSecure);
        var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException();
        }

        var user = await _userRepository.SingleOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            throw new KeyNotFoundException();
        }

        var response = GenerateUserToken(user);

        return response;
    }

    public async Task<AccessTokenResponseDto> LoginAsync(GoogleTokenRequestDto request)
    {
        throw new NotImplementedException();
    }

    private AccessTokenResponseDto GenerateUserToken(ApplicationUser user)
    {
        var tokenClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        };

        var refreshToken = GenerateJWTToken(tokenClaims, _jwtConfig.RefreshTokenExpiryMinute, _jwtConfig.RefreshTokenSecret);

        var roleClaim = new Claim(ClaimTypes.Role, user.Role.ToString());
        var usernameClaim = new Claim(ClaimTypes.Name, user.Username);

        tokenClaims.Add(roleClaim);
        tokenClaims.Add(usernameClaim);

        var accessToken = GenerateJWTToken(tokenClaims, _jwtConfig.TokenExpiryMinute, _jwtConfig.Secret);

        var response = new AccessTokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiredIn = Convert.ToInt32(TimeSpan.FromMinutes(_jwtConfig.TokenExpiryMinute).TotalSeconds)
        };

        return response;
    }

    private string GenerateJWTToken(IEnumerable<Claim> claims, int tokenExpiryMinute, string JWTSecretKey)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSecretKey));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtConfig.ValidIssuer,
            Audience = _jwtConfig.ValidAudience,
            Expires = DateTime.UtcNow.AddMinutes(tokenExpiryMinute),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}