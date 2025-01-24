using Template.Core.Repositories;
using Template.Core.Repositories.Interfaces;
using Template.Core.Storages;
using Template.Core.Storages.Interfaces;
using Template.Core.UnitOfWorks;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Service.Interfaces;
using Template.Service.src;

namespace Template.API.Configs;

public static class DIConfigureServices
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
        services.AddScoped<IErrorLogService, ErrorLogService>();
        services.AddSingleton<IStorageHelper, BlobStorageHelper>();
        services.AddScoped<ICourseRepository, CourseRepository>();

        return services;
    }
}