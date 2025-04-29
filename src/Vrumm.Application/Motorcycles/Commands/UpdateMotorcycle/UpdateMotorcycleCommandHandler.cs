using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Motorcycles.Commands.UpdateMotorcycle;
public class UpdateMotorcycleCommandHandler : ICommandHandler<UpdateMotorcycleCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMotorcycleCommandHandler> _logger;

    public UpdateMotorcycleCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateMotorcycleCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(UpdateMotorcycleCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating motorcycle {Id}", command.Id);

        var motorcycle = await _unitOfWork.Motorcycles.GetByIdAsync(command.Id, cancellationToken);
        if (motorcycle == null)
        {
            _logger.LogWarning("Motorcycle {Id} not found", command.Id);
            throw new NotFoundException("Motorcycle", command.Id);
        }

        if (command.LicensePlate != motorcycle.Details().LicensePlate().ToStringRepresentation() &&
            await _unitOfWork.Motorcycles.ExistsByLicensePlateAsync(command.LicensePlate, cancellationToken))
        {
            _logger.LogWarning("License plate {LicensePlate} is already in use", command.LicensePlate);
            throw new DuplicateLicensePlateException(command.LicensePlate);
        }

        var model = MotorcycleModel.Create(command.Model);
        var year = ManufactureYear.Create(command.Year);
        var licensePlate = LicensePlate.Create(command.LicensePlate);

        motorcycle.Update(model, year, licensePlate);

        await _unitOfWork.Motorcycles.UpdateAsync(motorcycle);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated motorcycle {Id}", command.Id);
    }
}