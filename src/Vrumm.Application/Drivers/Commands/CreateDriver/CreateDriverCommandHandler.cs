using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Domain.Exceptions;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Drivers.Commands.CreateDriver;
public class CreateDriverCommandHandler : ICommandHandler<CreateDriverCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateDriverCommandHandler> _logger;

    public CreateDriverCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateDriverCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Guid> Handle(CreateDriverCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating driver with CNPJ {Cnpj}", command.Cnpj);

        var cnpj = Cnpj.Create(command.Cnpj);
        var licenseNumber = LicenseNumber.Create(command.LicenseNumber);

        if (await _unitOfWork.Drivers.ExistsByCnpjAsync(cnpj, cancellationToken))
        {
            _logger.LogWarning("Driver with CNPJ {Cnpj} already exists", cnpj.Value);
            throw new DuplicateCnpjException(cnpj.Value);
        }

        if (await _unitOfWork.Drivers.ExistsByLicenseNumberAsync(licenseNumber, cancellationToken))
        {
            _logger.LogWarning("Driver with LicenseNumber {LicenseNumber} already exists", licenseNumber.ToStringRepresentation());
            throw new DomainException("License number must be unique.");
        }

        var driver = new Driver(
            command.Name,
            cnpj,
            BirthDate.Create(command.BirthDate),
            licenseNumber,
            LicenseTypeValue.Create(command.LicenseType));

        await _unitOfWork.Drivers.AddAsync(driver, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created driver with ID {DriverId}", driver.Id);

        return driver.Id;
    }
}