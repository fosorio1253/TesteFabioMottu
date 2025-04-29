namespace Vrumm.Application.Drivers.Commands.UploadLicense;
public class StorageOptions
{
    public string BucketName { get; set; }
    public int SignedUrlExpirationMinutes { get; set; } = 60;
}