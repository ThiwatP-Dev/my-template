using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Template.Core.Repositories.Interfaces;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Database.Models;
using Template.Service.Interfaces;

namespace Template.Service.src;

public class ErrorLogService(IUnitOfWork unitOfWork) : IErrorLogService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IGenericRepository<ErrorLog> _errorLogRepository = unitOfWork.Repository<ErrorLog>();
    private static readonly string[] s_fieldsToMask = [ "password" ];
    private static readonly string[] s_sensitiveHeaders = [ "Authorization", "x-api-key" ];
    
    public async Task LogAsync(HttpRequest request, string? stackTrace, string? message)
    {
        var requestBody = await GetRequestBodyAsync(request);

        var queryParams = GetQueryParameter(request);

        var headers = GetRequestHeader(request);

        var log = new ErrorLog
        {
            StackTrace = stackTrace,
            RequestPath = request.Path,
            RequestMethod = request.Method,
            RequestBody = requestBody,
            RequestHeaders = headers,
            QueryParams = queryParams,
            Message = message
        };

        await _errorLogRepository.CreateAsync(log);

        await _unitOfWork.SaveChangesAsync();
    }

    private static async Task<string> GetRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        request.Body.Position = 0;

         if (request.HasFormContentType)
        {
            // Handle form data
            var form = await request.ReadFormAsync();
            var formData = form.ToDictionary(
                x => x.Key,
                x => s_fieldsToMask.Contains(x.Key) ? "******" : x.Value.ToString()
            );

            // Serialize to JSON for consistent logging format
            return JsonConvert.SerializeObject(formData, Formatting.Indented);
        }

        // Handle JSON body
        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0; // Reset stream position for further usage

        if (string.IsNullOrWhiteSpace(body))
        {
            return body;
        }

        try
        {
            // Parse and mask JSON fields
            var jsonObject = JObject.Parse(body);
            foreach (var field in s_fieldsToMask)
            {
                if (jsonObject[field] != null)
                {
                    jsonObject[field] = "******";
                }
            }

            // Serialize back to a formatted JSON string
            return jsonObject.ToString(Formatting.Indented);
        }
        catch
        {
            // If parsing fails, return raw body
            return body;
        }
    }

    private static string GetRequestHeader(HttpRequest request)
    {
        var headers = request.Headers.ToDictionary(h => h.Key, 
            h => s_sensitiveHeaders.Contains(h.Key) ? "******" : h.Value.ToString());

        return string.Join("; ", headers);
    }

    private static string GetQueryParameter(HttpRequest request)
    {
        var queryParams = request.Query.Select(kvp => $"{kvp.Key}={kvp.Value}");

        return string.Join("&", queryParams);
    }
}