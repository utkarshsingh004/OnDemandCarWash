using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(
                exception,
                "An unhandled exception occurred. TraceId: {TraceId}",
                httpContext.TraceIdentifier);

            var (statusCode, title) = exception switch
            {
                ArgumentException =>
                    (StatusCodes.Status400BadRequest,
                    "Bad Request"),

                KeyNotFoundException =>
                    (StatusCodes.Status404NotFound,
                    "Resource Not Found"),

                UnauthorizedAccessException =>
                    (StatusCodes.Status401Unauthorized,
                    "Unauthorized"),

                _ =>
                    (StatusCodes.Status500InternalServerError,
                    "Internal Server Error")
            };

            var response = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = statusCode == StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred."
                    : exception.Message,
                Instance = httpContext.Request.Path
            };

            response.Extensions["traceId"] =
                httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType =
                "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(
                response,
                cancellationToken);

            return true;
        }
    }
}