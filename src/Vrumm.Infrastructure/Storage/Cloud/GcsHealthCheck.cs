using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Vrumm.Infrastructure.Storage.Cloud;
public class GcsHealthCheck : IHealthCheck
{
    private readonly CloudStorageOptions _options;
    private readonly ILogger<GcsHealthCheck> _logger;

    public GcsHealthCheck(IOptions<CloudStorageOptions> options, ILogger<GcsHealthCheck> logger)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var storageClient = await StorageClient.CreateAsync();

            // Try to get bucket metadata to check if bucket exists and is accessible
            var bucket = await storageClient.GetBucketAsync(_options.BucketName, cancellationToken: cancellationToken);

            return HealthCheckResult.Healthy($"Successfully connected to GCS bucket {_options.BucketName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed for GCS bucket {BucketName}", _options.BucketName);

            return new HealthCheckResult(
                context.Registration.FailureStatus,
                $"Failed to connect to GCS bucket {_options.BucketName}",
                ex);
        }
    }
}