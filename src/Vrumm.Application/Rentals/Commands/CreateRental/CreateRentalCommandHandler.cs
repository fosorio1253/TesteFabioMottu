using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Exceptions.Drivers;
using Vrumm.Domain.Exceptions.Motorcycles;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Messaging.Abstractions;

namespace Vrumm.Application.Rentals.Commands.CreateRental;
public class CreateRentalCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<CreateRentalCommandHandler> _logger;
    private readonly IDateTime _dateTime;

    public CreateRentalCommandHandler(
        IUnitOfWork unitOfWork,
        IMessagePublisher messagePublisher,
        ILogger<CreateRentalCommandHandler> logger,
        IDateTime dateTime)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
    }

    public async Task<Guid> Handle(CreateRentalCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Criando locação para motocicleta {MotorcycleId} e entregador {DriverId}",
            command.MotorcycleId, command.DriverId);

        var driver = await _unitOfWork.Drivers.GetByIdAsync(command.DriverId, cancellationToken);
        if (driver == null)
        {
            _logger.LogWarning("Entregador {DriverId} não encontrado", command.DriverId);
            throw new NotFoundException("Entregador", command.DriverId);
        }

        if (!driver.CanRentMotorcycle())
        {
            _logger.LogWarning("Entregador {DriverId} não possui habilitação válida para motos", command.DriverId);
            throw new InvalidDriverLicenseException("Entregador não possui habilitação válida para motos (tipo A ou AB)");
        }

        var motorcycle = await _unitOfWork.Motorcycles.GetByIdAsync(command.MotorcycleId, cancellationToken);
        if (motorcycle == null)
        {
            _logger.LogWarning("Motocicleta {MotorcycleId} não encontrada", command.MotorcycleId);
            throw new NotFoundException("Motocicleta", command.MotorcycleId);
        }

        var plan = await _unitOfWork.Plans.GetByIdAsync(command.PlanId, cancellationToken);
        if (plan == null)
        {
            _logger.LogWarning("Plano {PlanId} não encontrado", command.PlanId);
            throw new NotFoundException("Plano", command.PlanId);
        }

        if (await _unitOfWork.Rentals.HasActiveRentalForMotorcycleAsync(command.MotorcycleId, cancellationToken))
        {
            _logger.LogWarning("Motocicleta {MotorcycleId} com placa {LicensePlate} já possui uma locação ativa",
                command.MotorcycleId, motorcycle.Details().LicensePlate().ToStringRepresentation());
            throw new MotorcycleNotAvailableException("Motocicleta já está alugada.");
        }

        var rental = Rental.CreateNextDayRental(
            command.MotorcycleId,
            command.DriverId,
            command.PlanId,
            _dateTime.Now,
            plan);

        motorcycle.Rent();

        await _unitOfWork.Rentals.AddAsync(rental, cancellationToken);
        await _unitOfWork.Motorcycles.UpdateAsync(motorcycle);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Locação {RentalId} criada com sucesso para motocicleta com placa {LicensePlate}",
            rental.Id, motorcycle.Details().LicensePlate().ToStringRepresentation());

        var createdEvent = rental.GenerateCreatedEvent();
        await _messagePublisher.PublishAsync(createdEvent);

        return rental.Id;
    }
}