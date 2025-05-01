using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Motorcycles.Commands.DeleteMotorcycle;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Exceptions;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Xunit;

namespace Vrumm.Test.Unit.Application.Motorcycles.Commands;
public class DeleteMotorcycleCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<DeleteMotorcycleCommandHandler>> _loggerMock;
    private readonly DeleteMotorcycleCommandHandler _handler;

    public DeleteMotorcycleCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<DeleteMotorcycleCommandHandler>>();
        _handler = new DeleteMotorcycleCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_DeletesMotorcycle()
    {
        // Arrange
        var command = new DeleteMotorcycleCommand
        {
            Id = Guid.NewGuid()
        };

        var motorcycle = new Motorcycle(
            MotorcycleModel.Create("Honda CB500"),
            ManufactureYear.Create(2023),
            LicensePlate.Create("ABC1D23"));

        _unitOfWorkMock.Setup(u => u.Motorcycles.GetByIdAsync
        (command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);
        
        _unitOfWorkMock.Setup(u => u.Rentals.HasActiveRentalForMotorcycleAsync
        (command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _unitOfWorkMock.Setup(u => u.Motorcycles.RemoveAsync(motorcycle))
            .Returns(Task.CompletedTask);
        
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Motorcycles.RemoveAsync(motorcycle), Times.Once());
        
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync
        (It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_MotorcycleNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new DeleteMotorcycleCommand
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
    public async Task Handle_MotorcycleHasActiveRental_ThrowsDomainException()
    {
        // Arrange
        var command = new DeleteMotorcycleCommand
        {
            Id = Guid.NewGuid()
        };

        var motorcycle = new Motorcycle(
            MotorcycleModel.Create("Honda CB500"),
            ManufactureYear.Create(2023),
            LicensePlate.Create("ABC1D23"));

        _unitOfWorkMock.Setup(u => u.Motorcycles.GetByIdAsync
        (command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);
        
        _unitOfWorkMock.Setup(u => u.Rentals.HasActiveRentalForMotorcycleAsync
        (command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(()
            => _handler.Handle(command, CancellationToken.None));
    }
}