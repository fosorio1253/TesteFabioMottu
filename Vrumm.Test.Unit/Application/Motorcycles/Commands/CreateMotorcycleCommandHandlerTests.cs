using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Application.Motorcycles.Commands.CreateMotorcycle;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Domain.Events;
using Vrumm.Domain.Exceptions;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Messaging.Abstractions;
using Xunit;

namespace Vrumm.Test.Unit.Application.Motorcycles.Commands;
public class CreateMotorcycleCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<ILogger<CreateMotorcycleCommandHandler>> _loggerMock;
    private readonly CreateMotorcycleCommandHandler _handler;

    public CreateMotorcycleCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _publisherMock = new Mock<IMessagePublisher>();
        _loggerMock = new Mock<ILogger<CreateMotorcycleCommandHandler>>();
        _handler = new CreateMotorcycleCommandHandler
            (_unitOfWorkMock.Object, _publisherMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesMotorcycleAndPublishesEvent()
    {
        // Arrange
        var command = new CreateMotorcycleCommand
        {
            Id = Guid.NewGuid(),
            Model = "Honda CB500",
            Year = 2023,
            LicensePlate = "ABC1D23"
        };
        var motorcycle = new Motorcycle(
            MotorcycleModel.Create(command.Model),
            ManufactureYear.Create(command.Year),
            LicensePlate.Create(command.LicensePlate));

        _unitOfWorkMock.Setup(u => u.Motorcycles.ExistsByLicensePlateAsync
        (It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _unitOfWorkMock.Setup(u => u.Motorcycles.AddAsync
        (It.IsAny<Motorcycle>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        _publisherMock.Setup(p => p.PublishAsync
        (It.IsAny<MotorcycleRegistered>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(motorcycle.Id);
        
        _unitOfWorkMock.Verify(u => u.Motorcycles.AddAsync(
            It.Is<Motorcycle>(m =>
                m.Model() == command.Model &&
                m.Year() == command.Year &&
                m.LicensePlate() == command.LicensePlate),
            It.IsAny<CancellationToken>()), Times.Once());
        
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync
        (It.IsAny<CancellationToken>()), Times.Once());
        
        _publisherMock.Verify(p => p.PublishAsync
        (It.IsAny<MotorcycleRegistered>(), It.IsAny<string>()), Times.Once());
    }

    [Fact]
    public async Task Handle_DuplicateLicensePlate_ThrowsDomainException()
    {
        // Arrange
        var command = new CreateMotorcycleCommand
        {
            LicensePlate = "ABC1D23"
        };

        _unitOfWorkMock.Setup(u => u.Motorcycles.ExistsByLicensePlateAsync
        (It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(()
            => _handler.Handle(command, CancellationToken.None));
    }
}