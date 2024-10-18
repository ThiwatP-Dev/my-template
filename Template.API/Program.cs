using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Template.API.Authentication;
using Template.API.Configs;
using Template.Core.Configs;
using Template.Core.Constants;
using Template.Database;
using Template.Service.Profiles;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
services.AddControllers()
        .AddNewtonsoftJson(option => option.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ");

services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigurations.Secret))
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

services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(configuration.GetConnectionString("DatabaseContext"),
    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
services.AddConfigOption(configuration);
services.AddInjection();
services.AddClient();

services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .SetIsOriginAllowed(x => true);
    });
});

services.AddAutoMapper(typeof(InstituteProfile).Assembly);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dataContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseMiddleware<ExceptionMiddlewareHandler>();

app.UseAuthorization();

app.MapControllers();

app.Run();