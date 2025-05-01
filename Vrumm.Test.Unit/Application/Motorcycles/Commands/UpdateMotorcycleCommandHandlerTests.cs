using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Motorcycles.Commands.UpdateMotorcycle;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Exceptions;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Xunit;

namespace Vrumm.Test.Unit.Application.Motorcycles.Commands;
public class UpdateMotorcycleCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdateMotorcycleCommandHandler>> _loggerMock;
    private readonly UpdateMotorcycleCommandHandler _handler;

    public UpdateMotorcycleCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateMotorcycleCommandHandler>>();
        _handler = new UpdateMotorcycleCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesMotorcycle()
    {
        // Arrange
        var command = new UpdateMotorcycleCommand
        {
            Id = Guid.NewGuid(),
            Model = "Yamaha MT-07",
            Year = 2024,
            LicensePlate = "XYZ4E56"
        };

        var motorcycle = new Motorcycle(
            MotorcycleModel.Create("Honda CB500"),
            ManufactureYear.Create(2023),
            LicensePlate.Create("ABC1D23"));

        _unitOfWorkMock.Setup(u => u.Motorcycles.GetByIdAsync
        (command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);
        
        _unitOfWorkMock.Setup(u => u.Motorcycles.ExistsByLicensePlateAsync
        (It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _unitOfWorkMock.Setup(u => u.Motorcycles.UpdateAsync(It.IsAny<Motorcycle>()))
            .Returns(Task.CompletedTask);
        
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Motorcycles.UpdateAsync(It.Is<Motorcycle>(m =>
            m.Model() == command.Model &&
            m.Year() == command.Year &&
            m.LicensePlate() == command.LicensePlate)), Times.Once());
        
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync
        (It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_MotorcycleNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new UpdateMotorcycleCommand
        {
            Id = Guid.NewGuid()
        };
        
        _unitOfWorkMock.Setup(u => u.Motorcycles.GetByIdAsync
        (command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Motorcycle)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(()
            => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DuplicateLicensePlate_ThrowsDomainException()
    {
        // Arrange
        var command = new UpdateMotorcycleCommand
        {
            Id = Guid.NewGuid(),
            LicensePlate = "XYZ4E56"
        };
        var motorcycle = new Motorcycle(
            MotorcycleModel.Create("Honda CB500"),
            ManufactureYear.Create(2023),
            LicensePlate.Create("ABC1D23"));

        _unitOfWorkMock.Setup(u => u.Motorcycles.GetByIdAsync
        (command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);
        
        _unitOfWorkMock.Setup(u => u.Motorcycles.ExistsByLicensePlateAsync
        (It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(()
            => _handler.Handle(command, CancellationToken.None));
    }
}