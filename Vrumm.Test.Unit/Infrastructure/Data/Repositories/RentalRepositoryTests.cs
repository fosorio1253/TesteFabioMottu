using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities;
using Vrumm.Infrastructure.Data.Context;
using Vrumm.Infrastructure.Data.Repositories;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.Data.Repositories;
public class RentalRepositoryTests
{
    private readonly VrummDbContext _context;
    private readonly RentalRepository _repository;

    public RentalRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<VrummDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new VrummDbContext(options);
        _repository = new RentalRepository(_context);
    }

    [Fact]
    public async Task GetByDriverIdAsync_WhenRentalsExist_ReturnsMatchingRentals()
    {
        // Arrange
        var driverId = Guid.NewGuid();
        var rental1 = CreateRental(driverId: driverId);
        var rental2 = CreateRental(driverId: Guid.NewGuid());
        await _context.Rentals.AddRangeAsync(rental1, rental2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByDriverIdAsync(driverId, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().DriverId.Should().Be(driverId);
    }

    [Fact]
    public async Task GetByMotorcycleIdAsync_WhenRentalsExist_ReturnsMatchingRentals()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var rental = CreateRental(motorcycleId: motorcycleId);
        await _context.Rentals.AddAsync(rental);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByMotorcycleIdAsync
            (motorcycleId, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().MotorcycleId.Should().Be(motorcycleId);
    }

    [Fact]
    public async Task GetActiveRentalsAsync_WhenActiveRentalsExist_ReturnsOnlyActive()
    {
        // Arrange
        var activeRental = CreateRental(status: RentalStatus.Active);
        var finalizedRental = CreateRental(status: RentalStatus.Finalized);
        await _context.Rentals.AddRangeAsync(activeRental, finalizedRental);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveRentalsAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Status.Should().Be(RentalStatus.Active);
    }

    [Fact]
    public async Task HasActiveRentalForMotorcycleAsync_WhenActiveRentalExists_ReturnsTrue()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var rental = CreateRental(motorcycleId: motorcycleId, status: RentalStatus.Active);
        await _context.Rentals.AddAsync(rental);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HasActiveRentalForMotorcycleAsync
            (motorcycleId, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetByDateRangeAsync_WhenRentalsInRangeExist_ReturnsMatchingRentals()
    {
        // Arrange
        var rental = CreateRental
            (startDate: new DateTime(2023, 1, 1), expectedEndDate: new DateTime(2023, 1, 7));
        await _context.Rentals.AddAsync(rental);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByDateRangeAsync
            (new DateTime(2023, 1, 1), new DateTime(2023, 1, 10), CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    private Rental CreateRental(
        Guid? motorcycleId = null,
        Guid? driverId = null,
        RentalStatus? status = null,
        DateTime? startDate = null,
        DateTime? expectedEndDate = null)
    {
        var rental = new Rental(
            motorcycleId: motorcycleId ?? Guid.NewGuid(),
            driverId: driverId ?? Guid.NewGuid(),
            planId: 1,
            startDate: startDate ?? DateTime.Today,
            expectedEndDate: expectedEndDate ?? DateTime.Today.AddDays(7)
        );
        if (status == RentalStatus.Finalized)
        {
            rental.FinalizeRental(DateTime.Today.AddDays(7), 100);
        }
        else if (status == RentalStatus.Cancelled)
        {
            rental.CancelRental();
        }
        return rental;
    }
}