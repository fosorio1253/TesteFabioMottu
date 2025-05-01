using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Vrumm.Application.Plans.Queries.GetPlans;
using Vrumm.Domain.Entities;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Xunit;

namespace Vrumm.Test.Unit.Application.Plans.Queries;
public class GetPlansQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<GetPlansQueryHandler>> _loggerMock;
    private readonly GetPlansQueryHandler _handler;

    public GetPlansQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<GetPlansQueryHandler>>();
        _handler = new GetPlansQueryHandler(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidQuery_ReturnsPlans()
    {
        // Arrange
        var query = new GetPlansQuery();
        var plans = new List<Plan>
            {
                new Plan(1, 7, 30m, 0.2m, 40m),
                new Plan(2, 15, 28m, 0.4m, 50m)
            };

        _unitOfWorkMock.Setup(u => u.Plans.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(plans);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.First().DayCount.Should().Be(7);
        result.Items.First().DailyRate.Should().Be(30m);
        
        _unitOfWorkMock.Verify(u => u.Plans.GetAllAsync
        (It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_NoPlans_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetPlansQuery();
        _unitOfWorkMock.Setup(u => u.Plans.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Plan>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Items.Should().BeEmpty();
    }
}