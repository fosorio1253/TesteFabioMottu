using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Infrastructure.Data.Context;
using Vrumm.Infrastructure.Data.Repositories;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.Data.Repositories;
public class MotorcycleRepositoryTests
{
    private readonly VrummDbContext _context;
    private readonly MotorcycleRepository _repository;

    public MotorcycleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<VrummDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new VrummDbContext(options);
        _repository = new MotorcycleRepository(_context);
    }

    [Fact]
    public async Task ExistsByLicensePlateAsync_WhenMotorcycleExists_ReturnsTrue()
    {
        // Arrange
        var licensePlate = LicensePlate.Create("ABC1D23");
        var motorcycle = CreateMotorcycle(licensePlate: licensePlate);
        await _context.Motorcycles.AddAsync(motorcycle);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByLicensePlateAsync(licensePlate.ToStringRepresentation(), CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetByStatusAsync_WhenMotorcyclesExist_ReturnsMatchingMotorcycles()
    {
        // Arrange
        var motorcycle1 = CreateMotorcycle(status: MotorcycleStatus.Available);
        var motorcycle2 = CreateMotorcycle(status: MotorcycleStatus.Rented);
        await _context.Motorcycles.AddRangeAsync(motorcycle1, motorcycle2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByStatusAsync(MotorcycleStatus.Available, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Status().Should().Be(MotorcycleStatus.Available);
    }

    [Fact]
    public async Task GetByYearRangeAsync_WhenMotorcyclesExist_ReturnsMatchingMotorcycles()
    {
        // Arrange
        var motorcycle1 = CreateMotorcycle(year: ManufactureYear.Create(2020));
        var motorcycle2 = CreateMotorcycle(year: ManufactureYear.Create(2022));
        await _context.Motorcycles.AddRangeAsync(motorcycle1, motorcycle2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByYearRangeAsync(2020, 2021, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Year().Should().Be(2020);
    }

    private Motorcycle CreateMotorcycle(LicensePlate? licensePlate = null, ManufactureYear? year = null, MotorcycleStatus? status = null)
    {
        var motorcycle = new Motorcycle(
            MotorcycleModel.Create("Test Model"),
            year ?? ManufactureYear.Create(2020),
            licensePlate ?? LicensePlate.Create("ABC1D23")
        );
        if (status.HasValue && status != MotorcycleStatus.Available)
        {
            switch (status)
            {
                case MotorcycleStatus.Rented:
                    motorcycle.Rent();
                    break;
                case MotorcycleStatus.UnderMaintenance:
                    motorcycle.SetUnderMaintenance();
                    break;
                case MotorcycleStatus.Inactive:
                    motorcycle.SetInactive();
                    break;
            }
        }
        return motorcycle;
    }
}
