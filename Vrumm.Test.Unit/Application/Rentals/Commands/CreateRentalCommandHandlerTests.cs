using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Plans;
using Vrumm.Application.Rentals.Commands.CreateRental;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Events;
using Vrumm.Domain.Exceptions.Drivers;
using Vrumm.Domain.Exceptions.Motorcycles;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Messaging.Abstractions;
using Xunit;

namespace Vrumm.Test.Unit.Application.Rentals.Commands;
public class CreateRentalCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPlanFactory> _planFactoryMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<CreateRentalCommandHandler>> _loggerMock;
    private readonly CreateRentalCommandHandler _handler;

    public CreateRentalCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _planFactoryMock = new Mock<IPlanFactory>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<CreateRentalCommandHandler>>();

        _handler = new CreateRentalCommandHandler(
            _unitOfWorkMock.Object,
            _planFactoryMock.Object,
            _publisherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesRentalAndPublishesEvent()
    {
        // Arrange
        var command = new CreateRentalCommand
        {
            MotorcycleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            PlanId = 1,
            StartDate = DateTime.UtcNow.Date.AddDays(1),
            ExpectedEndDate = DateTime.UtcNow.Date.AddDays(8)
        };

        var motorcycle = new Motorcycle(
            MotorcycleModel.Create("Honda CB500"),
            ManufactureYear.Create(2023),
            LicensePlate.Create("ABC1D23"));
        var driver = new Driver(
            "John Doe",
            Cnpj.Create("12345678901234"),
            BirthDate.Create(DateTime.UtcNow.AddYears(-25)),
            LicenseNumber.Create("1234567890"),
            LicenseTypeValue.Create("A"),
            "license-image.png");
        var plan = new Plan(1, 7, 30m, 0.2m, 40m);
        var rental = Rental.CreateNextDayRental(
            command.MotorcycleId,
            command.DriverId,
            command.PlanId,
            command.StartDate,
            plan);

        _unitOfWorkMock.Setup(u => u.Motorcycles.GetByIdAsync
        (command.MotorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);
        
        _unitOfWorkMock.Setup(u => u.Drivers.GetByIdAsync
        (command.DriverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);
        
        _unitOfWorkMock.Setup(u => u.Rentals.HasActiveRentalForMotorcycleAsync
        (command.MotorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _planFactoryMock.Setup(p => p.CreatePlan(command.PlanId)).Returns(plan);
        
        _unitOfWorkMock.Setup(u => u.Rentals.AddAsync
        (It.IsAny<Rental>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        _unitOfWorkMock.Setup(u => u.Motorcycles.UpdateAsync(It.IsAny<Motorcycle>()))
            .Returns(Task.CompletedTask);
        
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        _publisherMock.Setup(p => p.PublishAsync
        (It.IsAny<MotorcycleRegistered>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(rental.Id);
        _unitOfWorkMock.Verify(u => u.Rentals.AddAsync(It.Is<Rental>(r =>
            r.MotorcycleId == command.MotorcycleId &&
            r.DriverId == command.DriverId &&
            r.PlanId == command.PlanId &&
            r.StartDate == command.StartDate), It.IsAny<CancellationToken>()), Times.Once());
        
        _unitOfWorkMock.Verify(u => u.Motorcycles.UpdateAsync
        (It.Is<Motorcycle>(m => m.Id == command.MotorcycleId)), Times.Once());

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync
        (It.IsAny<CancellationToken>()), Times.Once());
        
        _publisherMock.Verify(p => p.PublishAsync
        (It.IsAny<MotorcycleRegistered>(), It.IsAny<string>()), Times.Once());
    }

    [Fact]
    public async Task Handle_MotorcycleNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new CreateRentalCommand { MotorcycleId = Guid.NewGuid() };
        
        _unitOfWorkMock.Setup(u => u.Motorcycles.GetByIdAsync
        (command.MotorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Motorcycle)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(()
            => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DriverNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new CreateRentalCommand
        {
            MotorcycleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid()
        };
        
        var motorcycle = new Motorcycle(
            MotorcycleModel.Create("Honda CB500"),
            ManufactureYear.Create(2023),
            LicensePlate.Create("ABC1D23"));
        
        _unitOfWorkMock.Setup(u => u.Motorcycles.GetByIdAsync
        (command.MotorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);
        
        _unitOfWorkMock.Setup(u => u.Drivers.GetByIdAsync
        (command.DriverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Driver)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(()
            => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InvalidDriverLicense_ThrowsInvalidDriverLicenseException()
    {
        // Arrange
        var command = new CreateRentalCommand
        {
            MotorcycleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid()
        };
        
        var motorcycle = new Motorcycle(
            MotorcycleModel.Create("Honda CB500"),
            ManufactureYear.Create(2023),
            LicensePlate.Create("ABC1D23"));
        
        var driver = new Driver(
            "John Doe",
            Cnpj.Create("12345678901234"),
            BirthDate.Create(DateTime.UtcNow.AddYears(-25)),
            LicenseNumber.Create("1234567890"),
            LicenseTypeValue.Create("B"),
            "license-image.png");

        _unitOfWorkMock.Setup(u => u.Motorcycles.GetByIdAsync
        (command.MotorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);
        
        _unitOfWorkMock.Setup(u => u.Drivers.GetByIdAsync
        (command.DriverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDriverLicenseException>(()
            => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_MotorcycleAlreadyRented_ThrowsMotorcycleNotAvailableException()
    {
        // Arrange
        var command = new CreateRentalCommand
        {
            MotorcycleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid()
        };

        var motorcycle = new Motorcycle(
            MotorcycleModel.Create("Honda CB500"),
            ManufactureYear.Create(2023),
            LicensePlate.Create("ABC1D23"));

        var driver = new Driver(
            "John Doe",
            Cnpj.Create("12345678901234"),
            BirthDate.Create(DateTime.UtcNow.AddYears(-25)),
            LicenseNumber.Create("1234567890"),
            LicenseTypeValue.Create("A"),
            "license-image.png");

        _unitOfWorkMock.Setup(u => u.Motorcycles.GetByIdAsync
        (command.MotorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);
        
        _unitOfWorkMock.Setup(u => u.Drivers.GetByIdAsync
        (command.DriverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);
        
        _unitOfWorkMock.Setup(u => u.Rentals.HasActiveRentalForMotorcycleAsync
        (command.MotorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<MotorcycleNotAvailableException>(()
            => _handler.Handle(command, CancellationToken.None));
    }
}