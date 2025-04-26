using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Dependency;

namespace Vrumm.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        DatabaseDependencyInjection.AddDatabaseDependecies(services, hcBuilder);
        StorageDependencyInjection.AddStorageDependecies(services, hcBuilder);
        MessagingDependencyInjection.AddMessagingDependecies(services, hcBuilder);

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}