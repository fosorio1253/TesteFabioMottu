using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Application.Common.Models;
using Vrumm.Application.Motorcycles.Queries.GetMotorcycles;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Xunit;

namespace Vrumm.Test.Unit.Application.Motorcycles.Queries;
public class GetMotorcyclesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetMotorcyclesQueryHandler>> _loggerMock;
    private readonly GetMotorcyclesQueryHandler _handler;

    public GetMotorcyclesQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetMotorcyclesQueryHandler>>();
        _handler = new GetMotorcyclesQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsPagedMotorcycles()
    {
        // Arrange
        var query = new GetMotorcyclesQuery
        {
            LicensePlate = "ABC",
            PageNumber = 1,
            PageSize = 2,
            SortBy = "Model",
            SortDescending = false
        };

        var motorcycles = new List<Motorcycle>
            {
                new Motorcycle(
                    MotorcycleModel.Create("Honda CB500"),
                    ManufactureYear.Create(2023),
                    LicensePlate.Create("ABC1D23")),
                new Motorcycle(
                    MotorcycleModel.Create("Yamaha MT-07"),
                    ManufactureYear.Create(2024),
                    LicensePlate.Create("ABC4E56"))
            };

        _unitOfWorkMock.Setup(u => u.Motorcycles.GetQueryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycles.AsQueryable());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.First().Model.Should().Be("Honda CB500");
        result.TotalCount.Should().Be(2);
        result.PageIndex.Should().Be(query.PageNumber);
        result.TotalPages.Should().Be(query.PageSize);

        _unitOfWorkMock.Verify(u => u.Motorcycles.GetQueryAsync
        (It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPagedResult()
    {
        // Arrange
        var query = new GetMotorcyclesQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        var pagedResult = new PaginatedList<Motorcycle>(
            new List<Motorcycle>(),
            0,
            query.PageNumber,
            query.PageSize);

        var motorcycles = new List<Motorcycle>();

        _unitOfWorkMock.Setup(u => u.Motorcycles.GetQueryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycles.AsQueryable());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.PageIndex.Should().Be(query.PageNumber);
    }
}
