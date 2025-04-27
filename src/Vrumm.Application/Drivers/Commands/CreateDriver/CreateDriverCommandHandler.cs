using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities;
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
        _logger.LogInformation("Creating driver with Tax ID {TaxId}", command.TaxId);

        if (await _unitOfWork.Drivers.ExistsByTaxIdAsync(command.TaxId, cancellationToken))
        {
            _logger.LogWarning("Driver with Tax ID {TaxId} already exists", command.TaxId);
            throw new DuplicateTaxIdException(command.TaxId);
        }

        var driver = new Driver(
            command.Name,
            command.TaxId,
            command.BirthDate,
            command.LicenseNumber,
            Enum.Parse<LicenseType>(command.LicenseType));

        await _unitOfWork.Drivers.AddAsync(driver, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created driver with ID {DriverId}", driver.Id);

        return driver.Id;
    }
}