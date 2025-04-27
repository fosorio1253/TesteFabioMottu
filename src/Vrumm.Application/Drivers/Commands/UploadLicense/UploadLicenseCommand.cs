using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Drivers.Commands.UploadLicense;
public class UploadLicenseCommand : ICommand<string>
{
    public Guid DriverId { get; init; }
    public string FileName { get; init; }
    public string ContentType { get; init; }
    public Stream Content { get; init; }
}