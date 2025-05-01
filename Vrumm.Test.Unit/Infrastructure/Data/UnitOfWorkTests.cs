using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Domain.Repositories;
using Vrumm.Infrastructure.Data.Context;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.Data;
public class UnitOfWorkTests
{
    private readonly VrummDbContext _context;
    private readonly Mock<IMotorcycleRepository> _motorcycleRepositoryMock;
    private readonly Mock<IDriverRepository> _driverRepositoryMock;
    private readonly Mock<IPlanRepository> _planRepositoryMock;
    private readonly Mock<IRentalRepository> _rentalRepositoryMock;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<VrummDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new VrummDbContext(options);
        _motorcycleRepositoryMock = new Mock<IMotorcycleRepository>();
        _driverRepositoryMock = new Mock<IDriverRepository>();
        _planRepositoryMock = new Mock<IPlanRepository>();
        _rentalRepositoryMock = new Mock<IRentalRepository>();
        _unitOfWork = new UnitOfWork(_context, _motorcycleRepositoryMock.Object, _driverRepositoryMock.Object,
            _planRepositoryMock.Object, _rentalRepositoryMock.Object);
    }

    [Fact]
    public async Task SaveChangesAsync_UpdatesAuditFields()
    {
        // Arrange
        var driver = CreateDriver();
        await _context.Drivers.AddAsync(driver);
        var originalUpdateDate = driver.UpdateDate;

        // Act
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        // Assert
        driver.UpdateDate.Should().BeAfter(originalUpdateDate);
    }

    [Fact]
    public async Task BeginTransactionAsync_CreatesTransaction()
    {
        // Act
        await _unitOfWork.BeginTransactionAsync(CancellationToken.None);

        // Assert
        _context.Database.CurrentTransaction.Should().NotBeNull();
    }

    [Fact]
    public async Task CommitTransactionAsync_CommitsTransaction()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync(CancellationToken.None);
        var driver = CreateDriver();
        await _context.Drivers.AddAsync(driver);

        // Act
        await _unitOfWork.CommitTransactionAsync(CancellationToken.None);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        // Assert
        _context.Database.CurrentTransaction.Should().BeNull();
        (await _context.Drivers.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task RollbackTransactionAsync_RollsBackChanges()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync(CancellationToken.None);
        var driver = CreateDriver();
        await _context.Drivers.AddAsync(driver);

        // Act
        await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        // Assert
        _context.Database.CurrentTransaction.Should().BeNull();
        (await _context.Drivers.CountAsync()).Should().Be(0);
    }

    private Driver CreateDriver()
    {
        return new Driver(
            name: "Test Driver",
            cnpj: Cnpj.Create("12345678000195"),
            birthDate: BirthDate.Create(DateTime.Now.AddYears(-30)),
            licenseNumber: LicenseNumber.Create("123456789"),
            licenseType: LicenseTypeValue.Create("A"),
            licenseImageBase64: Convert.ToBase64String(new byte[] { 0x89, 0x50, 0x4E, 0x47 })
        );
    }
}