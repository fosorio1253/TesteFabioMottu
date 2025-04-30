using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Exceptions.Motorcycles;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Messaging.Abstractions;

namespace Vrumm.Application.Motorcycles.Commands.CreateMotorcycle;
public class CreateMotorcycleCommandHandler : ICommandHandler<CreateMotorcycleCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<CreateMotorcycleCommandHandler> _logger;

    public CreateMotorcycleCommandHandler(
        IUnitOfWork unitOfWork,
        IMessagePublisher messagePublisher,
        ILogger<CreateMotorcycleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork
            ?? throw new ArgumentNullException(nameof(unitOfWork));
        _messagePublisher = messagePublisher
            ?? throw new ArgumentNullException(nameof(messagePublisher));
        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> Handle
        (CreateMotorcycleCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating motorcycle with license plate {LicensePlate}",
            command.LicensePlate);

        if (await _unitOfWork.Motorcycles.ExistsByLicensePlateAsync
            (command.LicensePlate, cancellationToken))
        {
            _logger.LogWarning("Motorcycle with license plate {LicensePlate} already exists",
                command.LicensePlate);
            throw new DuplicateLicensePlateException(command.LicensePlate);
        }

        var motorcycle = new Motorcycle(
            MotorcycleModel.Create(command.Model),
            ManufactureYear.Create(command.Year),
            LicensePlate.Create(command.LicensePlate));

        await _unitOfWork.Motorcycles.AddAsync(motorcycle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created motorcycle {Id} with license plate {LicensePlate}",
            motorcycle.Id, motorcycle.LicensePlate());

        var registeredEvent = motorcycle.GenerateRegisteredEvent();
        await _messagePublisher.PublishAsync(registeredEvent);

        return motorcycle.Id;
    }
}