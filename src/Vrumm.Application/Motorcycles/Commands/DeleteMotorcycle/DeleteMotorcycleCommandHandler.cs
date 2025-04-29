using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Motorcycles.Commands.DeleteMotorcycle;
public class DeleteMotorcycleCommandHandler : ICommandHandler<DeleteMotorcycleCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteMotorcycleCommandHandler> _logger;

    public DeleteMotorcycleCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteMotorcycleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(DeleteMotorcycleCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting motorcycle {Id}", command.Id);

        var motorcycle = await _unitOfWork.Motorcycles.GetByIdAsync(command.Id, cancellationToken);
        if (motorcycle == null)
        {
            _logger.LogWarning("Motorcycle {Id} not found", command.Id);
            throw new NotFoundException("Motorcycle", command.Id);
        }

        if (motorcycle.Status().CanBeRemoved())
        {
            _logger.LogWarning("Motorcycle {Id} cannot be deleted (status: {Status})", command.Id, motorcycle.Status);
            throw new InvalidOperationException("Cannot delete a motorcycle that is not available.");
        }

        var rentals = await _unitOfWork.Rentals.GetByMotorcycleIdAsync(command.Id, cancellationToken);
        if (!motorcycle.CanBeDeleted(rentals))
        {
            _logger.LogWarning("Motorcycle {Id} cannot be deleted because it has rental history", command.Id);
            throw new MotorcycleHasRentalsException(command.Id);
        }

        await _unitOfWork.Motorcycles.RemoveAsync(motorcycle);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted motorcycle {Id}", command.Id);
    }
}