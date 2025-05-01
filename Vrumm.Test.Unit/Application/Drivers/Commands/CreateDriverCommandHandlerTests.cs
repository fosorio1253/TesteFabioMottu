using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Vrumm.Application.Drivers.Commands.CreateDriver;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Domain.Exceptions.Drivers;
using Vrumm.Domain.Exceptions;
using Vrumm.Domain.Options;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Storage.Abstractions;
using Xunit;
using FluentAssertions;

namespace Vrumm.Test.Unit.Application.Drivers.Commands;
public class CreateDriverCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IStorageService> _storageServiceMock;
    private readonly Mock<ILogger<CreateDriverCommandHandler>> _loggerMock;
    private readonly Mock<IOptions<GoogleCloudStorageOptions>> _storageOptionsMock;
    private readonly CreateDriverCommandHandler _handler;
    private readonly string _bucketName = "test-bucket";

    public CreateDriverCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _storageServiceMock = new Mock<IStorageService>();
        _loggerMock = new Mock<ILogger<CreateDriverCommandHandler>>();
        _storageOptionsMock = new Mock<IOptions<GoogleCloudStorageOptions>>();

        var storageOptions = new GoogleCloudStorageOptions { LicenseBucketName = _bucketName };
        _storageOptionsMock.Setup(o => o.Value).Returns(storageOptions);

        _handler = new CreateDriverCommandHandler(
            _storageOptionsMock.Object,
            _unitOfWorkMock.Object,
            _storageServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesDriverAndReturnsId()
    {
        // Arrange
        var command = new CreateDriverCommand
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Cnpj = "12345678901234",
            BirthDate = DateTime.UtcNow.AddYears(-25),
            LicenseNumber = "1234567890",
            LicenseType = "A",
            LicenseImageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII="
        };

        var cnpj = Cnpj.Create(command.Cnpj);
        var licenseNumber = LicenseNumber.Create(command.LicenseNumber);
        var driver = new Driver(
            command.Name,
            cnpj,
            BirthDate.Create(command.BirthDate),
            licenseNumber,
            LicenseTypeValue.Create(command.LicenseType),
            command.LicenseImageBase64);

        _unitOfWorkMock.Setup(u => u.Drivers.ExistsByCnpjAsync
        (cnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(u => u.Drivers.ExistsByLicenseNumberAsync
        (licenseNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(u => u.Drivers.AddAsync
        (It.IsAny<Driver>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var imageUrl = "https://storage.googleapis.com/test-bucket/licenses/driver.png";
        _storageServiceMock.Setup(s => s.UploadFileAsync(
            _bucketName,
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(imageUrl);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(driver.Id);
        _unitOfWorkMock.Verify(u => u.Drivers.AddAsync(It.Is<Driver>(d =>
            d.Name == command.Name &&
            d.Cnpj.Value == command.Cnpj &&
            d.LicenseNumber.ToStringRepresentation() == command.LicenseNumber &&
            d.LicenseImagePath == imageUrl), It.IsAny<CancellationToken>()), Times.Once());

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync
        (It.IsAny<CancellationToken>()), Times.Once());

        _storageServiceMock.Verify(s => s.UploadFileAsync(
            _bucketName,
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task Handle_DuplicateCnpj_ThrowsDuplicateCnpjException()
    {
        // Arrange
        var command = new CreateDriverCommand { Cnpj = "12345678901234" };
        var cnpj = Cnpj.Create(command.Cnpj);
        _unitOfWorkMock.Setup(u => u.Drivers.ExistsByCnpjAsync
        (cnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateCnpjException>(()
            => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DuplicateLicenseNumber_ThrowsDomainException()
    {
        // Arrange
        var command = new CreateDriverCommand
        {
            Cnpj = "12345678901234",
            LicenseNumber = "1234567890"
        };

        var cnpj = Cnpj.Create(command.Cnpj);
        var licenseNumber = LicenseNumber.Create(command.LicenseNumber);

        _unitOfWorkMock.Setup(u => u.Drivers.ExistsByCnpjAsync
        (cnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(u => u.Drivers.ExistsByLicenseNumberAsync
        (licenseNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(()
            => _handler.Handle(command, CancellationToken.None));
    }
}