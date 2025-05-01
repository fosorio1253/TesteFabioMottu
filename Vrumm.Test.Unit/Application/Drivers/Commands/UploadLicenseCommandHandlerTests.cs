using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Vrumm.Application.Common.Exceptions;
using Vrumm.Application.Drivers.Commands.UploadLicense;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Domain.Options;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Storage.Abstractions;
using Xunit;

namespace Vrumm.Test.Unit.Application.Drivers.Commands;
public class UploadLicenseCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IStorageService> _storageServiceMock;
    private readonly Mock<IOptions<GoogleCloudStorageOptions>> _storageOptionsMock;
    private readonly UploadLicenseCommandHandler _handler;
    private readonly string _bucketName = "test-bucket";

    public UploadLicenseCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _storageServiceMock = new Mock<IStorageService>();
        _storageOptionsMock = new Mock<IOptions<GoogleCloudStorageOptions>>();

        var storageOptions = new GoogleCloudStorageOptions { LicenseBucketName = _bucketName };
        _storageOptionsMock.Setup(o => o.Value).Returns(storageOptions);

        _handler = new UploadLicenseCommandHandler(
            _unitOfWorkMock.Object,
            _storageServiceMock.Object,
            _storageOptionsMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UploadsLicenseAndUpdatesDriver()
    {
        // Arrange
        var driver = new Driver(
            "John Doe",
            Cnpj.Create("12345678901234"),
            BirthDate.Create(DateTime.UtcNow.AddYears(-25)),
            LicenseNumber.Create("1234567890"),
            LicenseTypeValue.Create("A"),
            "old-license-image.png");

        var command = new UploadLicenseCommand
        {
            DriverId = driver.Id,
            FileName = "new-driver.png",
            ContentType = "image/png",
            Content = new MemoryStream(Convert.FromBase64String
            ("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAADUlEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII="))
        };

        _unitOfWorkMock.Setup(u => u.Drivers.GetByIdAsync
        (command.DriverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(driver);
        
        _unitOfWorkMock.Setup(u => u.Drivers.UpdateAsync(It.IsAny<Driver>()))
            .Returns(Task.CompletedTask);
        
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var imageUrl = "https://storage.googleapis.com/test-bucket/licenses/new-driver.png";
        _storageServiceMock.Setup(s => s.UploadFileAsync(
            _bucketName,
            It.IsAny<string>(),
            It.IsAny<Stream>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(imageUrl);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Drivers.UpdateAsync
        (It.Is<Driver>(d => d.LicenseImagePath == imageUrl)), Times.Once());
        
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
    public async Task Handle_DriverNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new UploadLicenseCommand { DriverId = Guid.NewGuid() };
        
        _unitOfWorkMock.Setup(u => u.Drivers.GetByIdAsync
        (command.DriverId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Driver)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(()
            => _handler.Handle(command, CancellationToken.None));
    }
}