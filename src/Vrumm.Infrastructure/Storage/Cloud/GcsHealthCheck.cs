using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vrumm.Infrastructure.Dependency.Configurations;

namespace Vrumm.Infrastructure.Storage.Cloud;
public class GcsHealthCheck : IHealthCheck
{
    private readonly GoogleCloudOptions _options;
    private readonly ILogger<GcsHealthCheck> _logger;

    public GcsHealthCheck(IOptions<GoogleCloudOptions> options, ILogger<GcsHealthCheck> logger)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var storageClient = await StorageClient.CreateAsync();

            storageClient.ListBuckets(_options.ProjectId);

            return HealthCheckResult.Healthy($"Successfully connected to GCS");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed for GCS");

            return new HealthCheckResult(
                context.Registration.FailureStatus,
                $"Failed to connect to GCS",
                ex);
        }
    }
}