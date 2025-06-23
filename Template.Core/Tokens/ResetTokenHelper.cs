using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Template.Core.Configs;
using Template.Core.Tokens.Interfaces;
using Template.Utility.Exceptions;

namespace Template.Core.Tokens;

public class ResetTokenHelper(IOptions<JWTConfiguration> jwtConfigOptions) : IResetTokenHelper
{
    private readonly JWTConfiguration _jwtConfig = jwtConfigOptions.Value;

    public (string Token, DateTime ExpiredAt) GenerateResetToken(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new("purpose", "reset_password"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiredAt = DateTime.UtcNow.AddMinutes(5);

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.ValidIssuer,
            audience: _jwtConfig.ValidAudience,
            claims: claims,
            expires: expiredAt,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiredAt);
    }

    public Guid ReadToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _jwtConfig.ValidIssuer,
            ValidateAudience = true,
            ValidAudience = _jwtConfig.ValidAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, parameters, out _);

            var purpose = principal.FindFirst("purpose")?.Value;
            if (purpose != "reset_password")
            {
                throw new CustomException.BadRequest("Invalid token purpose");
            }

            var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
            {
                throw new CustomException.BadRequest("Invalid user ID in token");
            }

            return userId;
        }
        catch (SecurityTokenExpiredException)
        {
            throw new CustomException.Unauthorized("Reset token has expired");
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Invalid reset token", ex);
        }
    }
}