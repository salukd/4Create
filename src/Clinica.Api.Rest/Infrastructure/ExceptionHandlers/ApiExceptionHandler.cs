using Clinica.Application.Common.Exceptions;

namespace Clinica.Api.Rest.Infrastructure.ExceptionHandlers;

public class ApiExceptionHandler : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger)
    {
        _logger = logger;
        _exceptionHandlers = new()
        {
            { typeof(ClinicaApiException), HandleClinicaApiExceptionAsync },
            { typeof(Exception), HandleUnknownExceptionAsync },
            { typeof(ClinicaApplicationException), HandleApplicationExceptionAsync },
        };
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out var handler))
        {
            await handler(httpContext, exception);
            return true;
        }

        _logger.LogError(exception, "Unhandled exception occurred");
        await HandleUnknownExceptionAsync(httpContext, exception);
        
        return false;
    }
    
    private async Task HandleClinicaApiExceptionAsync(HttpContext httpContext, Exception ex)
    {
        var exception = (ClinicaApiException)ex;
        _logger.LogError(ex, "Clinica API error occurred: {ErrorCode}", exception.ErrorCode);

        httpContext.Response.StatusCode = (int)exception.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = (int)exception.StatusCode,
            Type =
                $"https://tools.ietf.org/html/rfc7231#section-6.{(int)exception.StatusCode / 100}.{(int)exception.StatusCode % 100}",
            Title = "Clinica API Error",
            Detail = exception.Message
        });
    }

    private async Task HandleApplicationExceptionAsync(HttpContext httpContext, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception has occurred");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = exception.Message,
            Detail = exception.Message
        });
    }
    
    private async Task HandleUnknownExceptionAsync(HttpContext httpContext, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception has occurred");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "An error occurred while processing your request.",
            Detail = "An unexpected error occurred. Please try again later."
        });
    }
}