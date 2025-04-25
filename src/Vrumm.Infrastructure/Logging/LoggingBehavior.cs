using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Vrumm.Infrastructure.Logging;
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var requestName = typeof(TRequest).Name;
        var correlationId = Guid.NewGuid().ToString();

        _logger.LogInformation("[Start] {RequestName} {@CorrelationId}", requestName, correlationId);

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await next();
            stopwatch.Stop();

            _logger.LogInformation("[End] {RequestName} {@CorrelationId} completed in {ElapsedMilliseconds}ms",
                requestName, correlationId, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Error] {RequestName} {@CorrelationId} failed", requestName, correlationId);
            throw;
        }
    }
}