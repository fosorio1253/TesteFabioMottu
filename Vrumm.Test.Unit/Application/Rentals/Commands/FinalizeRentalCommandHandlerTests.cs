using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Rentals.Commands.FinalizeRental;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Exceptions;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Messaging.Abstractions;
using Xunit;
using Vrumm.Domain.Events;
using Vrumm.Domain.Common.Enums;

namespace Vrumm.Test.Unit.Application.Rentals.Commands;
public class FinalizeRentalCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<FinalizeRentalCommandHandler>> _loggerMock;
    private readonly FinalizeRentalCommandHandler _handler;

    public FinalizeRentalCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<FinalizeRentalCommandHandler>>();

        _handler = new FinalizeRentalCommandHandler(
            _unitOfWorkMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_FinalizesRentalAndPublishesEvent()
    {
        // Arrange
        var rental = new Rental(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            DateTime.UtcNow.Date.AddDays(-7),
            DateTime.UtcNow.Date);

        var command = new FinalizeRentalCommand
        {
            RentalId = rental.Id,
            ReturnDate = DateTime.UtcNow.Date
        };

        var plan = new Plan(1, 7, 30m, 0.2m, 40m);
        var motorcycle = new Motorcycle(
            MotorcycleModel.Create("Honda CB500"),
            ManufactureYear.Create(2023),
            LicensePlate.Create("ABC1D23"));

        _unitOfWorkMock.Setup(u => u.Rentals.GetByIdAsync
        (command.RentalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rental);

        _unitOfWorkMock.Setup(u => u.Plans.GetByIdAsync
        (rental.PlanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        _unitOfWorkMock.Setup(u => u.Motorcycles.GetByIdAsync
        (rental.MotorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);

        _unitOfWorkMock.Setup(u => u.Rentals.UpdateAsync(It.IsAny<Rental>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.Motorcycles.UpdateAsync(It.IsAny<Motorcycle>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _publisherMock.Setup(p => p.PublishAsync
        (It.IsAny<MotorcycleRegistered>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Rentals.UpdateAsync(It.Is<Rental>(r
            => r.Id == command.RentalId
            && r.EndDate == command.ReturnDate
            && r.Status == RentalStatus.Finalized)), Times.Once());

        _unitOfWorkMock.Verify(u => u.Motorcycles.UpdateAsync
        (It.Is<Motorcycle>(m => m.Id == rental.MotorcycleId)), Times.Once());

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync
        (It.IsAny<CancellationToken>()), Times.Once());

        _publisherMock.Verify(p => p.PublishAsync
        (It.IsAny<MotorcycleRegistered>(), It.IsAny<string>()), Times.Once());
    }

    [Fact]
    public async Task Handle_RentalNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new FinalizeRentalCommand { RentalId = Guid.NewGuid() };
        _unitOfWorkMock.Setup(u => u.Rentals.GetByIdAsync
        (command.RentalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Rental)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(()
            => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NonActiveRental_ThrowsDomainException()
    {
        // Arrange
        var command = new FinalizeRentalCommand
        {
            RentalId = Guid.NewGuid()
        };

        var rental = new Rental(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            DateTime.UtcNow.Date.AddDays(-7),
            DateTime.UtcNow.Date);

        rental.FinalizeRental(DateTime.UtcNow.Date, 0.2m);

        _unitOfWorkMock.Setup(u => u.Rentals.GetByIdAsync
        (command.RentalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rental);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(()
            => _handler.Handle(command, CancellationToken.None));
    }
}