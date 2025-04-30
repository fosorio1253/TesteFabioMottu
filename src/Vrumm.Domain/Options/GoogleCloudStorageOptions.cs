namespace Vrumm.Domain.Options;
public class GoogleCloudStorageOptions
{
    public const string SectionName = "GoogleCloud";
    public string LicenseBucketName { get; set; } = string.Empty;
    public int LicenseSignedUrlExpirationMinutes { get; set; } = 60;

    public string HealthCheckName { get; set; }
    public List<string> HealthCheckTags { get; set; }
}