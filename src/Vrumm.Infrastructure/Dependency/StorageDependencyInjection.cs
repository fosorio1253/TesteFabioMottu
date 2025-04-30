using Microsoft.Extensions.DependencyInjection;
using Vrumm.Domain.Options;
using Vrumm.Infrastructure.Storage.Abstractions;
using Vrumm.Infrastructure.Storage.GoogleCloud;

namespace Vrumm.Infrastructure.Dependency;
public static class StorageDependencyInjection
{
    public static IServiceCollection AddStorageDependecies(this IServiceCollection services, IHealthChecksBuilder hcBuilder,
        Action<GoogleCloudStorageOptions> configureOptions = null)
    {
        var storageOptions = new GoogleCloudStorageOptions();
        configureOptions?.Invoke(storageOptions);
        
        services.AddSingleton<IStorageService, GoogleCloudStorageService>();

        hcBuilder.AddCheck<GoogleCloudStorageHealthCheck>(
            storageOptions.HealthCheckName,
            tags: storageOptions.HealthCheckTags);

        return services;
    }
}