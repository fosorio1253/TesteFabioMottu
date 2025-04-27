using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Common.Behaviors;
public class LoggingBehavior<TRequest, TResult> : ICommandPipelineBehavior<TRequest, TResult>
        where TRequest : ICommand<TResult>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResult>> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResult>> logger,
        ICurrentUserService currentUserService,
        IDateTime dateTime)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
    }

    public async Task<TResult> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        CommandHandlerDelegate<TResult> next)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId ?? "Anonymous";
        var userName = _currentUserService.UserName ?? "Anonymous";

        _logger.LogInformation(
            "Handling command {RequestName} by user {UserId} ({UserName}) at {DateTime}",
            requestName, userId, userName, _dateTime.Now);

        try
        {
            var result = await next();
            _logger.LogInformation("Successfully handled command {RequestName}", requestName);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling command {RequestName}", requestName);
            throw;
        }
    }
}