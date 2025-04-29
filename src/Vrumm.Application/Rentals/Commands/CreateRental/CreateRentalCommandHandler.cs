using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Plans;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Exceptions.Drivers;
using Vrumm.Domain.Exceptions.Motorcycles;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Messaging.Abstractions;

namespace Vrumm.Application.Rentals.Commands.CreateRental;
public class CreateRentalCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPlanFactory _planFactory;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<CreateRentalCommandHandler> _logger;

    public CreateRentalCommandHandler(
        IUnitOfWork unitOfWork,
        IPlanFactory planFactory,
        IMessagePublisher publisher,
        ILogger<CreateRentalCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _planFactory = planFactory ?? throw new ArgumentNullException(nameof(planFactory));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(CreateRentalCommand command, CancellationToken cancellationToken)
    {
        var motorcycle = await _unitOfWork.Motorcycles.GetByIdAsync(command.MotorcycleId, cancellationToken)
            ?? throw new NotFoundException($"Motorcycle {command.MotorcycleId} not found.");

        var driver = await _unitOfWork.Drivers.GetByIdAsync(command.DriverId, cancellationToken)
            ?? throw new NotFoundException($"Driver {command.DriverId} not found.");

        if (!driver.CanRentMotorcycle())
            throw new InvalidDriverLicenseException("Driver does not have a valid license to rent a motorcycle.");

        if (await _unitOfWork.Rentals.HasActiveRentalForMotorcycleAsync(command.MotorcycleId, cancellationToken))
            throw new MotorcycleNotAvailableException("Motorcycle is already rented.");

        var plan = _planFactory.CreatePlan(command.PlanId);

        var rental = Rental.CreateNextDayRental(
            command.MotorcycleId,
            command.DriverId,
            command.PlanId,
            command.StartDate,
            plan);

        motorcycle.Rent();
        await _unitOfWork.Rentals.AddAsync(rental, cancellationToken);
        await _unitOfWork.Motorcycles.UpdateAsync(motorcycle);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var createdEvent = rental.GenerateCreatedEvent();
        await _publisher.PublishAsync(createdEvent);

        _logger.LogInformation("Rental {RentalId} created successfully.", rental.Id);
    }
}