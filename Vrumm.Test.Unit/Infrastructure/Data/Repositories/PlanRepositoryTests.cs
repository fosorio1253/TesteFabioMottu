using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Entities;
using Vrumm.Infrastructure.Data.Context;
using Vrumm.Infrastructure.Data.Repositories;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.Data.Repositories;
public class PlanRepositoryTests
{
    private readonly VrummDbContext _context;
    private readonly PlanRepository _repository;

    public PlanRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<VrummDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new VrummDbContext(options);
        
        _repository = new PlanRepository(_context);
    }

    [Fact]
    public async Task GetByDayCountRangeAsync_WhenPlansExist_ReturnsMatchingPlans()
    {
        // Arrange
        var plan1 = new Plan(1, 7, 30, 20, 50);
        var plan2 = new Plan(2, 15, 28, 40, 50);
        await _context.Plans.AddRangeAsync(plan1, plan2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByDayCountRangeAsync(7, 10, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().DayCount.Should().Be(7);
    }

    [Fact]
    public async Task SeedDefaultPlansAsync_SeedsPlansFromOptions()
    {
        // Arrange
        await _context.Plans.ExecuteDeleteAsync();

        // Act
        await _repository.SeedDefaultPlansAsync(CancellationToken.None);

        // Assert
        var plans = await _context.Plans.ToListAsync();
        plans.Should().HaveCount(2);
        plans.Should().Contain(p => p.DayCount == 7 && p.DailyRate == 30);
        plans.Should().Contain(p => p.DayCount == 15 && p.DailyRate == 28);
    }
}