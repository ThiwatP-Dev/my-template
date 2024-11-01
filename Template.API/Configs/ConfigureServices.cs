using Template.Core.Configs;
using Template.Core.Repositories;
using Template.Core.Repositories.Interfaces;
using Template.Core.UnitOfWorks;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Service.Clients;
using Template.Service.Interfaces;
using Template.Service.src;

namespace Template.API.Configs;

public static class ConfigureServices
{
    public static IServiceCollection AddInjection(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IInstituteService, InstituteService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ILearningPathService, LearningPathService>();

        return services;
    }

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
}