using Microsoft.AspNetCore.Http;

namespace Template.Service.Interfaces;

public interface IErrorLogService
{
    Task LogAsync(HttpRequest request, string? stackTrace, string? message);
}