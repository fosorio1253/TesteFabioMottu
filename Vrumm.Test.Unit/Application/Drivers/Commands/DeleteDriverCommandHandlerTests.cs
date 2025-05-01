using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Drivers.Commands.DeleteDriver;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Domain.Exceptions;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Xunit;

namespace Vrumm.Test.Unit.Application.Drivers.Commands;
public class DeleteDriverCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteDriverCommandHandler>> _loggerMock;
    private readonly DeleteDriverCommandHandler _handler;

    public DeleteDriverCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteDriverCommandHandler>>();
        _handler = new DeleteDriverCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_DeletesDriver()
    {
        // Arrange
        var command = new DeleteDriverCommand
        {
            Id = Guid.NewGuid()
        };

        var driver = new Driver(
            "John Doe",
            Cnpj.Create("12345678901234"),
            BirthDate.Create(DateTime.UtcNow.AddYears(-25)),
            LicenseNumber.Create("1234567890"),
            LicenseTypeValue.Create("A"),
            "license-image.png");

        _unitOfWorkMock.Setup(u => u.Drivers.GetByIdAsync
        (command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);
        
        _unitOfWorkMock.Setup(u => u.Rentals.HasActiveRentalForDriverAsync
        (command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _unitOfWorkMock.Setup(u => u.Drivers.RemoveAsync(driver))
            .Returns(Task.CompletedTask);
        
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Drivers.RemoveAsync(driver), Times.Once());
        
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync
        (It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_DriverNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new DeleteDriverCommand
        {
            Id = Guid.NewGuid()
        };

        _unitOfWorkMock.Setup(u => u.Drivers.GetByIdAsync
        (command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Driver)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(()
            => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DriverHasActiveRental_ThrowsDomainException()
    {
        // Arrange
        var command = new DeleteDriverCommand
        {
            Id = Guid.NewGuid()
        };

        var driver = new Driver(
            "John Doe",
            Cnpj.Create("12345678901234"),
            BirthDate.Create(DateTime.UtcNow.AddYears(-25)),
            LicenseNumber.Create("1234567890"),
            LicenseTypeValue.Create("A"),
            "license-image.png");

        _unitOfWorkMock.Setup(u => u.Drivers.GetByIdAsync
        (command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);
        
        _unitOfWorkMock.Setup(u => u.Rentals.HasActiveRentalForDriverAsync
        (command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(()
            => _handler.Handle(command, CancellationToken.None));
    }
}