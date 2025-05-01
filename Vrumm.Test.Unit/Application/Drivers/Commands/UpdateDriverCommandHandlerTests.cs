using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Drivers.Commands.UpdateDriver;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Xunit;

namespace Vrumm.Test.Unit.Application.Drivers.Commands;
public class UpdateDriverCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdateDriverCommandHandler>> _loggerMock;
    private readonly UpdateDriverCommandHandler _handler;

    public UpdateDriverCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateDriverCommandHandler>>();
        _handler = new UpdateDriverCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesDriver()
    {
        // Arrange
        var command = new UpdateDriverCommand
        {
            Id = Guid.NewGuid(),
            Name = "Jane Doe",
            BirthDate = DateTime.UtcNow.AddYears(-30)
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
        
        _unitOfWorkMock.Setup(u => u.Drivers.UpdateAsync(It.IsAny<Driver>()))
            .Returns(Task.CompletedTask);
        
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Drivers.UpdateAsync(It.Is<Driver>(d =>
            d.Name == command.Name &&
            d.BirthDate.Value == command.BirthDate)), Times.Once());
        
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync
        (It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_DriverNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new UpdateDriverCommand
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
}