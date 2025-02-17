using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Store.Middlewares;

public sealed class GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger) : IExceptionHandler, IMiddleware
{
    readonly ILogger<GlobalExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try { await next(context); }
        catch (Exception e)
        {
            await TryHandleAsync(context, e);
        }
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken = default)
    {
        var res = context.Response;

        ProblemDetails problem = new()
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Status = res.StatusCode = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Detail = exception.Message
        };

        await res.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
