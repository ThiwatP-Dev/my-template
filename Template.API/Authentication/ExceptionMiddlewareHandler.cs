using System.Net;

namespace Template.API.Authentication;

public class ExceptionMiddlewareHandler(RequestDelegate next, 
    IHostEnvironment env, 
    ILogger<ExceptionMiddlewareHandler> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly IHostEnvironment _env = env;
    private readonly ILogger<ExceptionMiddlewareHandler> _logger = logger;

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

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "An unhandled exception occurred.");

        var response = context.Response;
        var isProduction = _env.IsProduction();
        response.ContentType = "application/json";

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var result = new
        {
            code = "500C99",
            message = ex.Message,
            data = ex.Data,
            stackTrace = !isProduction ? ex.StackTrace : null
        };

        return context.Response.WriteAsJsonAsync(result);
    }
}