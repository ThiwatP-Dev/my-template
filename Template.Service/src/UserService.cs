using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Template.Core.Configs;
using Template.Core.Extensions;
using Template.Core.Repositories.Interfaces;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Database.Enums;
using Template.Database.Models;
using Template.Service.Dto;
using Template.Service.Dto.Authentication;
using Template.Service.Interfaces;

namespace Template.Service.src;

public class UserService(IUnitOfWork unitOfWork,
                         IOptions<JWTConfiguration> jwtConfigOptions) : IUserService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly JWTConfiguration _jwtConfig = jwtConfigOptions.Value;
    private readonly IGenericRepository<ApplicationUser> _userRepository = unitOfWork.Repository<ApplicationUser>();
    private readonly IGenericRepository<Learner> _learnerRepository = unitOfWork.Repository<Learner>();

    public async Task GenerateSuperAdmin()
    {
        if (await _userRepository.AnyAsync(x => x.Role == Role.SUPER_ADMIN))
        {
            return;
        }

        var hashKey = StringExtension.GenerateRandomString(10);
        var admin = new ApplicationUser(Role.SUPER_ADMIN)
        {
            Username = "super_admin",
            FirstName = "super_admin",
            HashedKey = hashKey,
            HashedPassword = "super_admin".HashHMACSHA256(hashKey)
        };

        await _unitOfWork.BeginTransactionAsync();

        await _userRepository.CreateAsync(admin);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
    }

    public async Task CreateLearnerAsync(CreateLearnerDto request, Guid userId)
    {
        var learnerExists = await _learnerRepository.AnyAsync(x => x.Email.Equals(request.Email));
        if (learnerExists)
        {
            throw new InvalidOperationException();
        }

        var code = await GetLearnerCode();
        var hashkey = StringExtension.GenerateRandomString(10);
        var learner = new Learner
        {
            Username = request.Email,
            HashedKey = hashkey,
            HashedPassword = request.Password.HashHMACSHA256(hashkey),
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Code = code,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            ProfileUrl = request.ProfileUrl
        };

        await _unitOfWork.BeginTransactionAsync();

        await _learnerRepository.CreateAsync(learner);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
    }

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

    private async Task<string> GetLearnerCode()
    {
        var count = await _learnerRepository.CountAsync();
        return $"{count:D10}";
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