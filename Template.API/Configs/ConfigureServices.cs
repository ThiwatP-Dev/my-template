using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Template.API.Authentication;
using Template.Core.Configs;
using Template.Core.Constants;
using Template.Service.Clients;
using Template.Service.Interfaces;

namespace Template.API.Configs;

public static class ConfigureServices
{
    public static IServiceCollection AddConfigOption(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<JWTConfiguration>(configuration.GetSection(JWTConfiguration.JWT));
        services.Configure<GoogleClientConfiguration>(settings =>
        {
            var section = configuration.GetSection(GoogleClientConfiguration.Client);
            settings.Audience = section.GetValue<string>(nameof(GoogleClientConfiguration.Audience))?.Split(",") ?? [];
        });

        return services;
    }

    public static IServiceCollection AddClient(this IServiceCollection services)
    {
        services.AddHttpClient();

        services.AddHttpClient<TemplateClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.example.com/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }

    public static IServiceCollection AddServiceAuthentication(this IServiceCollection services, ConfigurationManager configuration)
    {
        var jwtConfigurations = new JWTConfiguration();
        configuration.GetRequiredSection(JWTConfiguration.JWT)
            .Bind(jwtConfigurations);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = jwtConfigurations.ValidIssuer,
                ValidateAudience = true,
                ValidAudience = jwtConfigurations.ValidAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigurations.Secret)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var validator = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
                    var jti = context.Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", string.Empty);

                    if (jti is not null && await validator.IsTokenExpiredAsync(jti))
                    {
                        context.Fail("Token is blacklisted.");
                    }
                }
            };
        });

        var secretKeySettings = new SecretKeyConfiguration();
        configuration.GetRequiredSection(SecretKeyConfiguration.ConfigurationSettings)
                    .Bind(secretKeySettings);

        services.AddAuthentication(CustomAuthenticationSchemeConstant.ClientSecret)
                .AddScheme<SecretKeyAuthenticationSchemeOptions, SecretKeyAuthenticationHandler>(CustomAuthenticationSchemeConstant.ClientSecret, options =>
                {
                    options.SecretKey = secretKeySettings.Key;
                });

        services.AddAuthorizationBuilder()
                .SetDefaultPolicy(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser()
                                                                                                        .Build());

        return services;
    }
}