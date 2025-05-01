using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Rentals.Queries.CalculateReturnValue;
using Vrumm.Domain.Entities;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Xunit;

namespace Vrumm.Test.Unit.Application.Rentals.Queries;
public class CalculateReturnValueQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CalculateReturnValueQueryHandler>> _loggerMock;
    private readonly CalculateReturnValueQueryHandler _handler;

    public CalculateReturnValueQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CalculateReturnValueQueryHandler>>();
        _handler = new CalculateReturnValueQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsCalculatedValue()
    {
        // Arrange
        var query = new CalculateReturnValueQuery
        {
            RentalId = Guid.NewGuid(),
            ReturnDate = DateTime.UtcNow.Date
        };
        var rental = new Rental(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            DateTime.UtcNow.Date.AddDays(-7),
            DateTime.UtcNow.Date.AddDays(7));

        var plan = new Plan(1, 7, 30m, 0.2m, 40m);

        var calculatedValue = 210m; // 7 days * 30/day

        _unitOfWorkMock.Setup(u => u.Rentals.GetByIdAsync
        (query.RentalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rental);
        
        _unitOfWorkMock.Setup(u => u.Plans.GetByIdAsync
        (rental.PlanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(calculatedValue);
    }

    [Fact]
    public async Task Handle_RentalNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var query = new CalculateReturnValueQuery { RentalId = Guid.NewGuid() };
        _unitOfWorkMock.Setup(u => u.Rentals.GetByIdAsync(query.RentalId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Rental)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }
}