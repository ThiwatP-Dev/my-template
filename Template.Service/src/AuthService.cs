using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Template.Core.Configs;
using Template.Core.Repositories.Interfaces;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Database.Enums;
using Template.Database.Models;
using Template.Service.Dto.Authentication;
using Template.Service.Interfaces;
using Template.Utility.Exceptions;
using Template.Utility.Extensions;

namespace Template.Service.src;

public class AuthService(IUnitOfWork unitOfWork,
                         IOptions<JWTConfiguration> jwtConfigOptions,
                         IOptions<GoogleClientConfiguration> googleClientconfiguration) : IAuthService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly JWTConfiguration _jwtConfig = jwtConfigOptions.Value;
    private readonly GoogleClientConfiguration _googleClientConfig = googleClientconfiguration.Value;
    private readonly IGenericRepository<ApplicationUser> _userRepository = unitOfWork.Repository<ApplicationUser>();
    private readonly IGenericRepository<BlacklistedToken> _blacklistedTokenRepository = unitOfWork.Repository<BlacklistedToken>();

    public async Task<AccessTokenResponseDto> LoginAsync(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            throw new CustomException.BadRequest("Invalid username or password");
        }

        var user = await _userRepository.SingleOrDefaultAsync(x => x.Username.Equals(username), false);
        if (user is null || string.IsNullOrEmpty(user.HashedPassword))
        {
            throw new CustomException.NotFound("User not found");
        }

        var isPasswordMatched = password.IsHashHMACSHA256Match(user.HashedPassword, user.HashedKey);
        if (!isPasswordMatched)
        {
            throw new CustomException.Forbidden("Wrong password");
        }

        var response = GenerateUserToken(user);

        return response;
    }

    public async Task<AccessTokenResponseDto> RefreshAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
        {
            throw new CustomException.BadRequest("Invalid token");
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
            throw new CustomException.BadRequest("Invalid user claim");
        }

        var user = await _userRepository.SingleOrDefaultAsync(x => x.Id == userId, false);

        if (user is null)
        {
            throw new CustomException.NotFound("User not found");
        }

        var response = GenerateUserToken(user);

        return response;
    }

    public async Task<AccessTokenResponseDto> LoginGoogleAsync(string token)
    {
        token = token.Replace("Bearer ", string.Empty);
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = _googleClientConfig.Audience,
            IssuedAtClockTolerance = TimeSpan.FromSeconds(60)
        };

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

            if (payload is null)
            {
                throw new CustomException.BadRequest("Invalid payload");
            }

            var user = await _userRepository.SingleOrDefaultAsync(x => x.Username.Equals(payload.Email), false);
            if (user is null || string.IsNullOrEmpty(user.HashedPassword))
            {
                throw new CustomException.NotFound("User not found");
            }

            var response = GenerateUserToken(user);

            return response;
        }
        catch
        {
            throw;
        }
    }

    public async Task<AccessTokenResponseDto> LoginAzureADAsync(string token)
    {
        token = token.Replace("Bearer ", string.Empty);

        var jwtToken = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(token);

        var emailClaim = jwtToken.Claims.SingleOrDefault(x => x.Type == "email");
        var firstNameClaim = jwtToken.Claims.SingleOrDefault(x => x.Type == "given_name");
        var lastNameClaim = jwtToken.Claims.SingleOrDefault(x => x.Type == "family_name");

        if (emailClaim is null)
        {
            throw new CustomException.BadRequest("Invalid username");
        }

        var email = emailClaim.Value;
        var firstName = firstNameClaim?.Value ?? string.Empty;
        var lastName = lastNameClaim?.Value ?? string.Empty;

        var user = await _userRepository.SingleOrDefaultAsync(x => x.Username.Equals(email), false);

        if (user is null)
        {
            user = new ApplicationUser(Role.ADMIN)
            {
                Username = email,
                FirstName = firstName,
                LastName = lastName,
                HashedKey = string.Empty,   
                HashedPassword = string.Empty
            };

            await _unitOfWork.BeginTransactionAsync();

            await _userRepository.CreateAsync(user);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
        }

        var response = GenerateUserToken(user);

        return response;
    }

    public async Task LogoutAsync(string token)
    {
        token = token.Replace("Bearer ", string.Empty);
        if (await _blacklistedTokenRepository.AnyAsync(x => x.Token.Equals(token)))
        {
            return;
        }

        var blacklistedToken = new BlacklistedToken
        {
            Token = token
        };

        await _unitOfWork.BeginTransactionAsync();

        await _blacklistedTokenRepository.CreateAsync(blacklistedToken);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
    }

    public async Task<bool> IsTokenExpiredAsync(string token)
    {
        var isTokenExpired = await _blacklistedTokenRepository.AnyAsync(x => x.Token.Equals(token));

        return isTokenExpired;
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