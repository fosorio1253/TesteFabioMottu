using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Exceptions;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Rentals.Commands.CancelRental;
public class CancelRentalCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelRentalCommandHandler> _logger;

    public CancelRentalCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CancelRentalCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(CancelRentalCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelando locação {RentalId}", command.RentalId);

        var rental = await _unitOfWork.Rentals.GetByIdAsync(command.RentalId, cancellationToken);
        if (rental == null)
        {
            _logger.LogWarning("Locação {RentalId} não encontrada", command.RentalId);
            throw new NotFoundException("Locação", command.RentalId);
        }

        if (rental.Status != RentalStatus.Active)
        {
            _logger.LogWarning("Locação {RentalId} não está ativa (status: {Status})", command.RentalId, rental.Status);
            throw new DomainException("Não é possível cancelar uma locação não ativa.");
        }

        var motorcycle = await _unitOfWork.Motorcycles.GetByIdAsync(rental.MotorcycleId, cancellationToken);
        if (motorcycle == null)
        {
            _logger.LogWarning("Motocicleta {MotorcycleId} não encontrada para locação {RentalId}",
                rental.MotorcycleId, command.RentalId);
            throw new NotFoundException("Motocicleta", rental.MotorcycleId);
        }

        rental.CancelRental();

        motorcycle.ReturnMotorcycle();

        await _unitOfWork.Rentals.UpdateAsync(rental);
        await _unitOfWork.Motorcycles.UpdateAsync(motorcycle);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Locação {RentalId} cancelada com sucesso", command.RentalId);
    }
}