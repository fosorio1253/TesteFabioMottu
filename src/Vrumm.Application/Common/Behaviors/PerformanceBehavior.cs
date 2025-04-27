using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Common.Behaviors;
public class PerformanceBehavior<TRequest, TResult> : ICommandPipelineBehavior<TRequest, TResult>
        where TRequest : ICommand<TResult>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResult>> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly Stopwatch _stopwatch;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResult>> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _stopwatch = new Stopwatch();
    }

    public async Task<TResult> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        CommandHandlerDelegate<TResult> next)
    {
        _stopwatch.Start();

        try
        {
            return await next();
        }
        finally
        {
            _stopwatch.Stop();
            var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userId = _currentUserService.UserId ?? "Anonymous";
                var userName = _currentUserService.UserName ?? "Anonymous";

                _logger.LogWarning(
                    "Long running command: {RequestName} took {ElapsedMilliseconds}ms by user {UserId} ({UserName})",
                    requestName, elapsedMilliseconds, userId, userName);
            }
        }
    }
}