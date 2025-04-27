using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Exceptions;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Drivers.Commands.DeleteDriver;
public class DeleteDriverCommandHandler : ICommandHandler<DeleteDriverCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteDriverCommandHandler> _logger;

    public DeleteDriverCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteDriverCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(DeleteDriverCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Excluindo entregador {DriverId}", command.Id);

        var driver = await _unitOfWork.Drivers.GetByIdAsync(command.Id, cancellationToken);
        if (driver == null)
            throw new NotFoundException("Entregador", command.Id);

        var activeRentals = await _unitOfWork.Rentals.GetByDriverIdAsync(command.Id, cancellationToken);
        if (activeRentals.Any(r => r.Status == Domain.Common.Enums.RentalStatus.Active))
            throw new DomainException("Não é possível excluir um entregador com locações ativas.");

        await _unitOfWork.Drivers.RemoveAsync(driver);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Entregador {DriverId} excluído com sucesso", command.Id);
    }
}