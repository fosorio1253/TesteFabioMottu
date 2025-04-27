using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Events;

namespace Vrumm.Application.Rentals.Events;
public class RentalCreatedNotificationHandler : INotificationHandler<RentalCreated>
{
    private readonly ILogger<RentalCreatedNotificationHandler> _logger;

    public RentalCreatedNotificationHandler(ILogger<RentalCreatedNotificationHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(RentalCreated notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processando evento de criação de locação {RentalId}", notification.RentalId);
        _logger.LogInformation("Evento de criação de locação {RentalId} processado", notification.RentalId);
        return Task.CompletedTask;
    }
}