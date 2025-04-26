using Microsoft.Extensions.DependencyInjection;
using Vrumm.Infrastructure.Storage.Abstractions;
using Vrumm.Infrastructure.Storage.Cloud;

namespace Vrumm.Infrastructure.Dependency;
public static class StorageDependencyInjection
{
    public static IServiceCollection AddStorageDependecies(this IServiceCollection services, IHealthChecksBuilder hcBuilder,
        Action<StorageOptions> configureOptions = null)
    {
        var storageOptions = new StorageOptions();
        configureOptions?.Invoke(storageOptions);
        
        services.AddSingleton<IStorageService, CloudStorageService>();

        hcBuilder.AddCheck<GcsHealthCheck>(
            storageOptions.GoogleCloudStorage.Name,
            tags: storageOptions.GoogleCloudStorage.Tags);

        return services;
    }

    public class StorageOptions
    {
        public GoogleCloudStorageOptions GoogleCloudStorage { get; set; } = new GoogleCloudStorageOptions();

        public class GoogleCloudStorageOptions
        {
            public string Name { get; set; }
            public List<string> Tags { get; set; }
        }
    }
}