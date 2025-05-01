using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Infrastructure.Data.Context;
using Vrumm.Infrastructure.Data.Repositories;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.Data.Repositories;
public class DriverRepositoryTests
{
    private readonly VrummDbContext _context;
    private readonly DriverRepository _repository;

    public DriverRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<VrummDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new VrummDbContext(options);
        _repository = new DriverRepository(_context);
    }

    [Fact]
    public async Task ExistsByCnpjAsync_WhenDriverExists_ReturnsTrue()
    {
        // Arrange
        var cnpj = Cnpj.Create("12345678000195");
        var driver = CreateDriver(cnpj: cnpj);
        await _context.Drivers.AddAsync(driver);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByCnpjAsync(cnpj, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByCnpjAsync_WhenDriverDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var cnpj = Cnpj.Create("12345678000195");

        // Act
        var result = await _repository.ExistsByCnpjAsync(cnpj, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByLicenseNumberAsync_WhenLicenseExists_ReturnsTrue()
    {
        // Arrange
        var licenseNumber = LicenseNumber.Create("123456789");
        var driver = CreateDriver(licenseNumber: licenseNumber);
        await _context.Drivers.AddAsync(driver);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByLicenseNumberAsync
            (licenseNumber, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByCnpjExceptIdAsync_WhenDifferentDriverExists_ReturnsTrue()
    {
        // Arrange
        var cnpj = Cnpj.Create("12345678000195");
        var driver1 = CreateDriver(id: Guid.NewGuid(), cnpj: cnpj);
        var driver2 = CreateDriver(id: Guid.NewGuid());
        await _context.Drivers.AddRangeAsync(driver1, driver2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository
            .ExistsByCnpjAsync(cnpj, cancellationToken: CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task GetByCnpjAsync_WhenDriverExists_ReturnsDriver()
    {
        // Arrange
        var cnpj = Cnpj.Create("12345678000195");
        var driver = CreateDriver(cnpj: cnpj);
        await _context.Drivers.AddAsync(driver);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCnpjAsync(cnpj, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Cnpj.Should().Be(cnpj);
    }

    [Fact]
    public async Task GetByLicenseTypeAsync_WhenDriversExist_ReturnsMatchingDrivers()
    {
        // Arrange
        var licenseType = LicenseTypeValue.Create("A");
        var driver1 = CreateDriver(licenseType: licenseType);
        var driver2 = CreateDriver(licenseType: LicenseTypeValue.Create("AB"));
        await _context.Drivers.AddRangeAsync(driver1, driver2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository
            .GetByLicenseTypeAsync(licenseType, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    private Driver CreateDriver(
        Guid? id = null,
        Cnpj? cnpj = null,
        LicenseNumber? licenseNumber = null,
        LicenseTypeValue? licenseType = null)
    {
        return new Driver(
            name: "Test Driver",
            cnpj: cnpj ?? Cnpj.Create("12345678000195"),
            birthDate: BirthDate.Create(DateTime.Now.AddYears(-30)),
            licenseNumber: licenseNumber ?? LicenseNumber.Create("123456789"),
            licenseType: licenseType ?? LicenseTypeValue.Create("A"),
            licenseImageBase64: Convert.ToBase64String(new byte[] { 0x89, 0x50, 0x4E, 0x47 })
        );
    }
}