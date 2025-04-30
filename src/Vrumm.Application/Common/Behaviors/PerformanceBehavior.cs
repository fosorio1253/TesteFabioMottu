using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Options;

namespace Vrumm.Application.Common.Behaviors;
public class PerformanceBehavior : ICommandBus
{
    private readonly ICommandBus _next;
    private readonly ILogger<PerformanceBehavior> _logger;
    private readonly Stopwatch _stopwatch;
    private readonly int _performanceThresholdMs;

    public PerformanceBehavior(
        ICommandBus next,
        ILogger<PerformanceBehavior> logger,
        IOptions<PerformanceOptions> performanceOptions)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _stopwatch = new Stopwatch();
        _performanceThresholdMs = performanceOptions.Value?.ThresholdMs ?? 500;
    }


    public async Task<TResult> DispatchCommand<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
    {
        _stopwatch.Restart();
        try
        {
            return await _next.DispatchCommand(command, cancellationToken);
        }
        finally
        {
            _stopwatch.Stop();
            LogPerformanceIfSlow(command.GetType().Name, _stopwatch.ElapsedMilliseconds);
        }
    }

    public async Task<TResponse> DispatchQuery<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken)
    {
        _stopwatch.Restart();
        try
        {
            return await _next.DispatchQuery<TResponse>(query, cancellationToken);
        }
        finally
        {
            _stopwatch.Stop();
            LogPerformanceIfSlow(query.GetType().Name, _stopwatch.ElapsedMilliseconds);
        }
    }

    private void LogPerformanceIfSlow(string requestName, long elapsedMilliseconds)
    {
        if (elapsedMilliseconds > _performanceThresholdMs)
        {
            _logger.LogWarning(
                "Long running request: {RequestName} took {ElapsedMilliseconds}ms (threshold: {ThresholdMs}ms)",
                requestName,
                elapsedMilliseconds,
                _performanceThresholdMs);
        }
    }
}