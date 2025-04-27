using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Events;

namespace Vrumm.Application.Rentals.Events;
public class RentalFinalizedNotificationHandler : INotificationHandler<RentalFinalized>
{
    private readonly ILogger<RentalFinalizedNotificationHandler> _logger;

    public RentalFinalizedNotificationHandler(ILogger<RentalFinalizedNotificationHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(RentalFinalized message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processando evento de finalização de locação {RentalId}", message.RentalId);

        _logger.LogInformation("Evento de finalização de locação {RentalId} processado", message.RentalId);
        return Task.CompletedTask;
    }
}