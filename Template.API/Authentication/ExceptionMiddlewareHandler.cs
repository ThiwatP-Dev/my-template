using System.Net;
using FluentValidation;
using Mailjet.Client.Resources;
using Template.Service.Interfaces;
using Template.Utility.Exceptions;

namespace Template.API.Authentication;

public class ExceptionMiddlewareHandler(RequestDelegate next,
    IHostEnvironment env,
    IServiceProvider serviceProvider)
{
    private readonly RequestDelegate _next = next;
    private readonly IHostEnvironment _env = env;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var request = context.Request;
        
        using var scope = _serviceProvider.CreateScope();
        var logService = scope.ServiceProvider.GetRequiredService<IErrorLogService>();

        await logService.LogAsync(request, ex.StackTrace, ex.Message);

        var response = context.Response;
        var isProduction = _env.IsProduction();
        var message = ex.Message;
        object? data = null;
        response.ContentType = "application/json";
        context.Response.ContentType = "application/json";

        if (ex is CustomException customException)
        {
            context.Response.StatusCode = (int)customException.StatusCode;
            data = customException.CustomData;
        }
        if (ex is ValidationException validationException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            message = validationException.Errors.ElementAtOrDefault(0)?.ErrorMessage ?? "Bad request.";
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        var result = new
        {
            message,
            stackTrace = !isProduction ? ex.StackTrace : null,
            data
        };

        await context.Response.WriteAsJsonAsync(result);
    }
}