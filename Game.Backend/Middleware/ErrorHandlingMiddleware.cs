using Game.Backend.Shared.Errors;

namespace Game.Backend.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException exception)
        {
            await WriteErrorAsync(context, exception.StatusCode, exception.Code, exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled backend exception.");
            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "internal_error",
                "An unexpected backend error occurred.");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string code,
        string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new ErrorEnvelope(new ErrorDetails(code, message)));
    }
}
