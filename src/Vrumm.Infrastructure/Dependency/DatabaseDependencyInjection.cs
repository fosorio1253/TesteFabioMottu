using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vrumm.Domain.Repositories;
using Vrumm.Infrastructure.Data.Context;
using Vrumm.Infrastructure.Data.Repositories;

namespace Vrumm.Infrastructure.Dependency;
public static class DatabaseDependencyInjection
{
    public static IServiceCollection AddDatabaseDependecies
        (this IServiceCollection services, IHealthChecksBuilder hcBuilder,
        Action<DatabaseOptions> configureOptions = null)
    {
        var dataBaseOptions = new DatabaseOptions();
        configureOptions?.Invoke(dataBaseOptions);

        services.AddDbContext<VrummDbContext>(options =>
        {
            options.UseNpgsql(dataBaseOptions.PostgreDb.ConnectionString,
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(VrummDbContext).Assembly.FullName));
        });

        services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<IMotorcycleRegistrationEventRepository, MotorcycleRegistrationEventRepository>();

        hcBuilder.AddNpgSql(
            dataBaseOptions.PostgreDb.ConnectionString,
            name: dataBaseOptions.PostgreDb.Name,
            tags: dataBaseOptions.PostgreDb.Tags);

        return services;
    }

    public class DatabaseOptions
    {
        public PostgreOptions PostgreDb { get; set; } = new PostgreOptions();

        public class PostgreOptions
        {
            public string ConnectionString { get; set; }
            public string Name { get; set; }
            public List<string> Tags { get; set; }
        }
    }
}