using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Vrumm.Application.Common.Models;
using Vrumm.Application.Rentals.Queries.GetRentals;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Xunit;

namespace Vrumm.Test.Unit.Application.Rentals.Queries;
public class GetRentalsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetRentalsQueryHandler>> _loggerMock;
    private readonly GetRentalsQueryHandler _handler;

    public GetRentalsQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetRentalsQueryHandler>>();
        _handler = new GetRentalsQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsPagedRentals()
    {
        // Arrange
        var query = new GetRentalsQuery
        {
            DriverId = Guid.NewGuid(),
            PageNumber = 1,
            PageSize = 2,
            SortBy = "StartDate",
            SortDescending = true
        };

        var rentals = new List<Rental>
        {
            new Rental(
                Guid.NewGuid(),
                query.DriverId.Value,
                1,
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(1)),
            new Rental(
                Guid.NewGuid(),
                query.DriverId.Value,
                2,
                DateTime.UtcNow.AddDays(-2),
                DateTime.UtcNow.AddDays(2))
        };

        _unitOfWorkMock.Setup(u => u.Rentals.GetQueryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentals.AsQueryable());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.First().DriverId.Should().Be(query.DriverId.Value);
        result.TotalCount.Should().Be(2);
        result.PageIndex.Should().Be(query.PageNumber);

        _unitOfWorkMock.Verify(u => u.Rentals.GetQueryAsync
        (It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPagedResult()
    {
        // Arrange
        var query = new GetRentalsQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        var rentals = new List<Rental>();

        _unitOfWorkMock.Setup(u => u.Rentals.GetQueryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentals.AsQueryable());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.PageIndex.Should().Be(query.PageNumber);
    }
}