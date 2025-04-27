using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Exceptions;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Rentals.Queries.CalculateReturnValue;
public class CalculateReturnValueQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CalculateReturnValueQueryHandler> _logger;

    public CalculateReturnValueQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<CalculateReturnValueQueryHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<decimal> Handle(CalculateReturnValueQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calculando valor de devolução para locação {RentalId} com data {ReturnDate}",
            query.RentalId, query.ReturnDate);

        var rental = await _unitOfWork.Rentals.GetByIdAsync(query.RentalId, cancellationToken);
        if (rental == null)
        {
            _logger.LogWarning("Locação {RentalId} não encontrada", query.RentalId);
            throw new NotFoundException("Locação", query.RentalId);
        }

        if (rental.Status != RentalStatus.Active)
        {
            _logger.LogWarning("Locação {RentalId} não está ativa (status: {Status})", query.RentalId, rental.Status);
            throw new DomainException("Não é possível calcular valor para uma locação não ativa.");
        }

        var plan = await _unitOfWork.Plans.GetByIdAsync(rental.PlanId, cancellationToken);
        if (plan == null)
        {
            _logger.LogWarning("Plano {PlanId} não encontrado para locação {RentalId}", rental.PlanId, query.RentalId);
            throw new NotFoundException("Plano", rental.PlanId);
        }

        var returnValue = rental.CalculateReturnValue(query.ReturnDate, plan);

        _logger.LogInformation("Valor de devolução calculado para locação {RentalId}: {ReturnValue}",
            query.RentalId, returnValue);

        return returnValue;
    }
}