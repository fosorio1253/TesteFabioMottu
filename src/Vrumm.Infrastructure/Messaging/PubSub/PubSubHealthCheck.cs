using Google.Api.Gax.ResourceNames;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vrumm.Infrastructure.Dependency.Configurations;

namespace Vrumm.Infrastructure.Messaging.PubSub;
public class PubSubHealthCheck : IHealthCheck
{
    private readonly GoogleCloudOptions _options;
    private readonly ILogger<PubSubHealthCheck> _logger;

    public PubSubHealthCheck(IOptions<GoogleCloudOptions> options, ILogger<PubSubHealthCheck> logger)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var publisherServiceApiClient = await PublisherServiceApiClient.CreateAsync(cancellationToken);

            var projectName = new ProjectName(_options.ProjectId);
            var pageableResponse = publisherServiceApiClient.ListTopics(projectName);

            pageableResponse.ReadPage(1);

            return HealthCheckResult.Healthy($"Successfully connected to Pub/Sub project {_options.ProjectId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed for Pub/Sub project {ProjectId}", _options.ProjectId);

            return new HealthCheckResult(
                context.Registration.FailureStatus,
                $"Failed to connect to Pub/Sub project {_options.ProjectId}",
                ex);
        }
    }
}