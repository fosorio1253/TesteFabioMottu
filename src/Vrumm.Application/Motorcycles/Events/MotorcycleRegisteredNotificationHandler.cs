using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Events;

namespace Vrumm.Application.Motorcycles.Events;
public class MotorcycleRegisteredNotificationHandler : INotificationHandler<MotorcycleRegistered>
{
    private readonly ILogger<MotorcycleRegisteredNotificationHandler> _logger;

    public MotorcycleRegisteredNotificationHandler(ILogger<MotorcycleRegisteredNotificationHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(MotorcycleRegistered message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing motorcycle registered event for motorcycle {MotorcycleId}", message.MotorcycleId);
        _logger.LogInformation("Processed motorcycle registered event for motorcycle {MotorcycleId}", message.MotorcycleId);
        return Task.CompletedTask;
    }
}