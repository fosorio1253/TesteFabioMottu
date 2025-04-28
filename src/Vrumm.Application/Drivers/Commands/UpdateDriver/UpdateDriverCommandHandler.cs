using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Domain.Exceptions.Drivers;
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

        var cnpj = Cnpj.Create(command.TaxId);
        var licenseNumber = LicenseNumber.Create(command.LicenseNumber);

        if (await _unitOfWork.Drivers.ExistsByCnpjExceptIdAsync(cnpj, command.Id, cancellationToken))
            throw new DuplicateDriverException(command.TaxId);

        if (await _unitOfWork.Drivers.ExistsByLicenseNumberExceptIdAsync(licenseNumber, command.Id, cancellationToken))
            throw new DuplicateLicenseNumberException(command.LicenseNumber);

        var birthDate = BirthDate.Create(command.BirthDate);
        var licenseType = LicenseTypeValue.Create(command.LicenseType);

        driver.Update(
            name: command.Name,
            cnpj: cnpj,
            birthDate: birthDate,
            licenseNumber: licenseNumber,
            licenseType: licenseType);

        await _unitOfWork.Drivers.UpdateAsync(driver);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Entregador {DriverId} atualizado com sucesso", command.Id);
    }
}