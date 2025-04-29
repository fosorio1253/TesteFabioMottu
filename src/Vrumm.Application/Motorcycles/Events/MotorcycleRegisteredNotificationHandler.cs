using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Application.Motorcycles.Policies;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Events;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Motorcycles.Events;
public class MotorcycleRegisteredNotificationHandler
{
    private readonly DateTime _dateTime;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMotorcycleRegistrationPolicy _policy;
    private readonly ILogger<MotorcycleRegisteredNotificationHandler> _logger;

    public MotorcycleRegisteredNotificationHandler(
        IDateTime dateTime,
        IUnitOfWork unitOfWork,
        IMotorcycleRegistrationPolicy policy,
        ILogger<MotorcycleRegisteredNotificationHandler> logger)
    {
        _dateTime = dateTime.Now;
        _unitOfWork = unitOfWork;
        _policy = policy;
        _logger = logger;
    }

    public async Task Handle(MotorcycleRegistered notification, CancellationToken cancellationToken)
    {
        if (!_policy.ShouldRegisterEvent(notification))
        {
            _logger.LogInformation("Skipping registration for motorcycle {MotorcycleId} (Year: {Year})",
                notification.MotorcycleId, notification.Year);
            return;
        }

        if (await _unitOfWork.MotorcycleRegistrationEvents.ExistsByMotorcycleIdAsync(notification.MotorcycleId, _dateTime, cancellationToken: cancellationToken))
        {
            _logger.LogInformation("Event already processed for motorcycle {MotorcycleId}", notification.MotorcycleId);
            return;
        }

        var registrationEvent = new MotorcycleRegistrationEvent(
            notification.MotorcycleId,
            notification.Year,
            notification.Model,
            notification.LicensePlate,
            _dateTime
        );

        await _unitOfWork.MotorcycleRegistrationEvents.AddAsync(registrationEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Registered event for motorcycle {MotorcycleId}", notification.MotorcycleId);
    }
}