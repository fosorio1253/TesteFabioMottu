using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Application.Motorcycles.Queries.GetMotorcycleRegistrationEvents;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Xunit;

namespace Vrumm.Test.Unit.Application.Drivers.Queries;
public class GetMotorcycleRegistrationEventsHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetMotorcycleRegistrationEventsHandler>> _loggerMock;
    private readonly GetMotorcycleRegistrationEventsHandler _handler;

    public GetMotorcycleRegistrationEventsHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetMotorcycleRegistrationEventsHandler>>();
        _handler = new GetMotorcycleRegistrationEventsHandler
            (_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsRegistrationEvents()
    {
        // Arrange
        var query = new GetMotorcycleRegistrationEventsQuery
        {
            Year = 2023
        };

        var events = new List<MotorcycleRegistrationEvent>
            {
                new MotorcycleRegistrationEvent(
                    Guid.NewGuid(),
                    2023,
                    "honda",
                    "ABC123",
                    DateTime.UtcNow.AddDays(-1)),
                new MotorcycleRegistrationEvent(
                    Guid.NewGuid(),
                    2023,
                    "honda",
                    "ABC124",
                    DateTime.UtcNow.AddDays(-1)),
            };

        _unitOfWorkMock.Setup(u => u.MotorcycleRegistrationEvents.GetQueryAsync
        (It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.AsQueryable());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        
        _unitOfWorkMock.Verify(u => u.Motorcycles.GetQueryAsync
        (It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_NoEvents_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetMotorcycleRegistrationEventsQuery
        {
            Year = 2023
        };

        var events = new List<MotorcycleRegistrationEvent>();

        _unitOfWorkMock.Setup(u => u.MotorcycleRegistrationEvents.GetQueryAsync
        (It.IsAny<CancellationToken>()))
            .ReturnsAsync(events.AsQueryable());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}