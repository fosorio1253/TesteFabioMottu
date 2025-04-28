using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Events;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Motorcycles.Events;
public class MotorcycleRegisteredNotificationHandler : INotificationHandler<MotorcycleRegistered>
{
    private readonly ILogger<MotorcycleRegisteredNotificationHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public MotorcycleRegisteredNotificationHandler(
        ILogger<MotorcycleRegisteredNotificationHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task Handle(MotorcycleRegistered notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Processing MotorcycleRegistered event for motorcycle {MotorcycleId}, Model: {Model}, Year: {Year}, LicensePlate: {LicensePlate}",
                notification.MotorcycleId, notification.Model, notification.Year, notification.LicensePlate);

            bool eventExists = await _unitOfWork.MotorcycleRegistrationEvents.ExistsByMotorcycleIdAsync(
                notification.MotorcycleId, notification.Timestamp, cancellationToken);

            if (eventExists)
            {
                _logger.LogInformation(
                    "Event for motorcycle {MotorcycleId} at {Timestamp} has already been processed. Skipping.",
                    notification.MotorcycleId, notification.Timestamp);
                return;
            }

            if (notification.Year == 2024)
            {
                _logger.LogInformation(
                    "Motorcycle {MotorcycleId} is from 2024. Creating registration event record.",
                    notification.MotorcycleId);

                var registrationEvent = new MotorcycleRegistrationEvent(
                    notification.MotorcycleId,
                    notification.Year,
                    notification.Model,
                    notification.LicensePlate,
                    notification.Timestamp);

                await _unitOfWork.MotorcycleRegistrationEvents.AddAsync(registrationEvent, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Successfully stored registration event for 2024 motorcycle {MotorcycleId} with event ID {EventId}",
                    notification.MotorcycleId, registrationEvent.Id);
            }
            else
            {
                _logger.LogInformation(
                    "Motorcycle {MotorcycleId} is not from 2024 (Year: {Year}). Skipping storage.",
                    notification.MotorcycleId, notification.Year);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing MotorcycleRegistered event for motorcycle {MotorcycleId}: {ErrorMessage}",
                notification.MotorcycleId, ex.Message);
            throw;
        }
    }
}