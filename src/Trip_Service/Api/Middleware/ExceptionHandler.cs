using Microsoft.AspNetCore.Diagnostics;

namespace Trip_Service.Api.Middleware;

public sealed class ExceptionHandler(ILogger<ExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            HttpRequestException => StatusCodes.Status502BadGateway,
            _ => StatusCodes.Status500InternalServerError
        };

        logger.LogError(exception, "Request failed with status code {StatusCode}", statusCode);
        httpContext.Response.StatusCode = statusCode;

        var error = new
        {
            message = statusCode == StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : exception.Message
        };

        await httpContext.Response.WriteAsJsonAsync(error, cancellationToken);
        return true;
    }
}
