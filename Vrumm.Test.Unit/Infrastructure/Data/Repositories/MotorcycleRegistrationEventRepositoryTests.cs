using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Infrastructure.Data.Context;
using Vrumm.Infrastructure.Data.Repositories;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.Data.Repositories;
public class MotorcycleRegistrationEventRepositoryTests
{
    private readonly VrummDbContext _context;
    private readonly MotorcycleRegistrationEventRepository _repository;

    public MotorcycleRegistrationEventRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<VrummDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new VrummDbContext(options);
        _repository = new MotorcycleRegistrationEventRepository(_context);
    }

    [Fact]
    public async Task ExistsByMotorcycleIdAsync_WhenEventExists_ReturnsTrue()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var @event = CreateEvent(motorcycleId: motorcycleId);
        await _context.MotorcycleRegistrationEvents.AddAsync(@event);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByMotorcycleIdAsync(motorcycleId, @event.EventTimestamp, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetByYearAsync_WhenEventsExist_ReturnsMatchingEvents()
    {
        // Arrange
        var @event = CreateEvent(year: 2023);
        await _context.MotorcycleRegistrationEvents.AddAsync(@event);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByYearAsync(2023, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Year.Should().Be(2023);
    }

    [Fact]
    public async Task AddAsync_AddsEvent()
    {
        // Arrange
        var @event = CreateEvent();

        // Act
        await _repository.AddAsync(@event, CancellationToken.None);
        await _context.SaveChangesAsync();

        // Assert
        var savedEvent = await _context.MotorcycleRegistrationEvents.FirstOrDefaultAsync();
        savedEvent.Should().NotBeNull();
        savedEvent!.Model.Should().Be("Test Model");
    }

    private MotorcycleRegistrationEvent CreateEvent(Guid? motorcycleId = null, int? year = null)
    {
        return new MotorcycleRegistrationEvent(
            motorcycleId: motorcycleId ?? Guid.NewGuid(),
            year: year ?? 2023,
            model: "Test Model",
            licensePlate: "ABC1D23",
            eventTimestamp: DateTime.UtcNow
        );
    }
}