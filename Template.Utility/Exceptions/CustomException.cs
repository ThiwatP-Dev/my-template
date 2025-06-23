using System.Net;

namespace Template.Utility.Exceptions;

public class CustomException(HttpStatusCode statusCode, string? message = null, object? data = null) : Exception($"{message}.")
{
    public HttpStatusCode StatusCode { get; set; } = statusCode;

    public object? CustomData { get; set; } = data;

    public class NotFound(string? message = null, object? data = null) : CustomException(HttpStatusCode.NotFound, message, data) { }

    public class Conflict(string? message = null, object? data = null) : CustomException(HttpStatusCode.Conflict, message, data) { }

    public class BadRequest(string? message = null, object? data = null) : CustomException(HttpStatusCode.BadRequest, message, data) { }

    public class Forbidden(string? message = null, object? data = null) : CustomException(HttpStatusCode.Forbidden, message, data) { }

    public class Unauthorized(string? message = null, object? data = null) : CustomException(HttpStatusCode.Unauthorized, message, data) { }
}