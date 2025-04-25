using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Vrumm.Infrastructure.Middleware;
public class CorrelationIdMiddleware
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        var correlationId = GetOrCreateCorrelationId(context);

        // Add or update the correlation ID in the response headers
        context.Response.OnStarting(() =>
        {
            context.Response.Headers.Add(CorrelationIdHeaderName, new[] { correlationId });
            return Task.CompletedTask;
        });

        // Add correlation ID to log context
        using (_logger.BeginScope("{CorrelationId}", correlationId))
        {
            _logger.LogInformation("Request {Method} {Path} started", context.Request.Method, context.Request.Path);

            try
            {
                await _next(context);

                _logger.LogInformation("Request {Method} {Path} completed with status code {StatusCode}",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request {Method} {Path} failed",
                    context.Request.Method, context.Request.Path);
                throw;
            }
        }
    }

    private string GetOrCreateCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out StringValues correlationId);

        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        return correlationId;
    }
}