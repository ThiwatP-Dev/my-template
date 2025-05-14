using System.Text;
using Mailjet.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
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
        services.Configure<BlobStorageConfiguration>(configuration.GetSection(BlobStorageConfiguration.Blob));
        services.Configure<LineConfiguration>(configuration.GetSection(LineConfiguration.Line)); 
        services.Configure<GoogleClientConfiguration>(settings =>
        {
            var section = configuration.GetSection(GoogleClientConfiguration.Client);
            settings.Audience = section.GetValue<string>(nameof(GoogleClientConfiguration.Audience))?.Split(",") ?? [];
        });
        services.Configure<MailjetConfiguration>(configuration.GetSection(MailjetConfiguration.Mailjet));

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

        services.AddHttpClient<LineClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.line.me/");
        });

        services.AddHttpClient<IMailjetClient, MailjetClient>((serviceProvider, client) =>
        {
            client.SetDefaultSettings();
            
            var options = serviceProvider.GetRequiredService<IOptions<MailjetConfiguration>>().Value;
            client.UseBasicAuthentication(options.ApiKey, options.ApiSecret);
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
        })
        .AddMicrosoftIdentityWebApi(configuration, AzureADConfiguration.AzureAD, AzureADConfiguration.AzureAD);

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