using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Application.Drivers.Queries.GetDrivers;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Xunit;

namespace Vrumm.Test.Unit.Application.Drivers.Queries;
public class GetDriversQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetDriversQueryHandler>> _loggerMock;
    private readonly GetDriversQueryHandler _handler;

    public GetDriversQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetDriversQueryHandler>>();
        _handler = new GetDriversQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsPagedDrivers()
    {
        // Arrange
        var query = new GetDriversQuery
        {
            Filter = new DriverFilter()
            {
                Name = "John",
            },
            PageNumber = 1,
            PageSize = 2,
            SortBy = "Name",
            SortDescending = false
        };

        var drivers = new List<Driver>
            {
                new Driver(
                    "John Doe",
                    Cnpj.Create("12345678901234"),
                    BirthDate.Create(DateTime.UtcNow.AddYears(-25)),
                    LicenseNumber.Create("1234567890"),
                    LicenseTypeValue.Create("A"),
                    "license.png"),
                new Driver(
                    "John Smith",
                    Cnpj.Create("98765432109876"),
                    BirthDate.Create(DateTime.UtcNow.AddYears(-30)),
                    LicenseNumber.Create("0987654321"),
                    LicenseTypeValue.Create("A"),
                    "license2.png")
            };

        _unitOfWorkMock.Setup(u => u.Drivers.GetQueryAsync(
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(drivers.AsQueryable());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.First().Name.Should().Be("John Doe");
        result.TotalCount.Should().Be(2);
        result.PageIndex.Should().Be(query.PageNumber);

        _unitOfWorkMock.Verify(u => u.Drivers.GetQueryAsync
        (It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyPagedResult()
    {
        // Arrange
        var query = new GetDriversQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        var drivers = new List<Driver>();

        _unitOfWorkMock.Setup(u => u.Drivers.GetQueryAsync
        (It.IsAny<CancellationToken>()))
            .ReturnsAsync(drivers.AsQueryable());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.PageIndex.Should().Be(query.PageNumber);
    }
}