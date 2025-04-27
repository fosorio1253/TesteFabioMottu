using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Drivers.Commands.UpdateDriver;
public class UpdateDriverCommandHandler : ICommandHandler<UpdateDriverCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateDriverCommandHandler> _logger;

    public UpdateDriverCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateDriverCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(UpdateDriverCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Atualizando entregador {DriverId}", command.Id);

        var driver = await _unitOfWork.Drivers.GetByIdAsync(command.Id, cancellationToken);
        if (driver == null)
            throw new NotFoundException("Entregador", command.Id);

        if (await _unitOfWork.Drivers.ExistsByTaxIdExceptIdAsync(command.TaxId, command.Id, cancellationToken))
            throw new DuplicateEntityException($"Já existe um entregador com o CPF {command.TaxId}");

        if (await _unitOfWork.Drivers.ExistsByLicenseNumberExceptIdAsync(command.LicenseNumber, command.Id, cancellationToken))
            throw new DuplicateEntityException($"Já existe um entregador com o número de licença {command.LicenseNumber}");

        driver.Update(
            name: command.Name,
            taxId: command.TaxId,
            birthDate: command.BirthDate,
            licenseNumber: command.LicenseNumber,
            licenseType: command.LicenseType);

        await _unitOfWork.Drivers.UpdateAsync(driver);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Entregador {DriverId} atualizado com sucesso", command.Id);
    }
}