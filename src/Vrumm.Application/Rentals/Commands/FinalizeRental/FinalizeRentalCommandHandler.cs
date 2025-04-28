using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Exceptions;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Messaging.Abstractions;

namespace Vrumm.Application.Rentals.Commands.FinalizeRental;
public class FinalizeRentalCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<FinalizeRentalCommandHandler> _logger;

    public FinalizeRentalCommandHandler(
        IUnitOfWork unitOfWork,
        IMessagePublisher messagePublisher,
        ILogger<FinalizeRentalCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(FinalizeRentalCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Finalizando locação {RentalId} com data de devolução {ReturnDate}",
            command.RentalId, command.ReturnDate);

        var rental = await _unitOfWork.Rentals.GetByIdAsync(command.RentalId, cancellationToken);
        if (rental == null)
        {
            _logger.LogWarning("Locação {RentalId} não encontrada", command.RentalId);
            throw new NotFoundException("Locação", command.RentalId);
        }

        if (rental.Status != RentalStatus.Active)
        {
            _logger.LogWarning("Locação {RentalId} não está ativa (status: {Status})", command.RentalId, rental.Status);
            throw new DomainException("Não é possível finalizar uma locação não ativa.");
        }

        var plan = await _unitOfWork.Plans.GetByIdAsync(rental.PlanId, cancellationToken);
        if (plan == null)
        {
            _logger.LogWarning("Plano {PlanId} não encontrado para locação {RentalId}", rental.PlanId, command.RentalId);
            throw new NotFoundException("Plano", rental.PlanId);
        }

        var motorcycle = await _unitOfWork.Motorcycles.GetByIdAsync(rental.MotorcycleId, cancellationToken);
        if (motorcycle == null)
        {
            _logger.LogWarning("Motocicleta {MotorcycleId} não encontrada para locação {RentalId}",
                rental.MotorcycleId, command.RentalId);
            throw new NotFoundException("Motocicleta", rental.MotorcycleId);
        }

        var totalValue = rental.CalculateReturnValue(command.ReturnDate, plan);

        rental.FinalizeRental(command.ReturnDate, totalValue);

        motorcycle.Return();

        await _unitOfWork.Rentals.UpdateAsync(rental);
        await _unitOfWork.Motorcycles.UpdateAsync(motorcycle);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Locação {RentalId} finalizada com valor total {TotalValue}",
            command.RentalId, totalValue);

        var finalizedEvent = rental.GenerateFinalizedEvent();
        await _messagePublisher.PublishAsync(finalizedEvent);
    }
}