using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Template.Core;
using Template.Core.Emails;
using Template.Core.Emails.Interfaces;
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
        services.AddScoped<BackgroundJobService>();
        services.AddScoped<EmailHelper, SMTPHelper>();
        services.AddScoped<IEncryptedRecordService, EncryptedRecordService>();
        services.AddSingleton(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            var projectId = config["Firebase:ProjectId"];
            var credentialPath = Path.Combine(AppContext.BaseDirectory, "firebase-app.json");

            return FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(credentialPath),
                ProjectId = projectId
            }, projectId);
        });
        
        services.AddSingleton(provider =>
        {
            var app = provider.GetRequiredService<FirebaseApp>();
            return FirebaseMessaging.GetMessaging(app);
        });
        
        services.AddScoped<FirebaseHelper>();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}