using System.Net;

namespace Template.Utility.Exceptions;

public class CustomException(HttpStatusCode statusCode, string? message = null) : Exception($"{message}.")
{
    public HttpStatusCode StatusCode { get; set; } = statusCode;

    public class NotFound(string? message = null) : CustomException(HttpStatusCode.NotFound, message) { }

    public class Conflict(string? message = null) : CustomException(HttpStatusCode.Conflict, message) { }

    public class BadRequest(string? message = null) : CustomException(HttpStatusCode.BadRequest, message) { }

    public class Forbidden(string? message = null) : CustomException(HttpStatusCode.Forbidden, message) { }
}