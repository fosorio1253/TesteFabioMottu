using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vrumm.Application.Common.Behaviors;
using Vrumm.Application.Common.CommandBus;
using Vrumm.Application.Common.Dispatching;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Application.Common.Models;
using Vrumm.Application.Drivers.Commands.CreateDriver;
using Vrumm.Application.Drivers.Commands.DeleteDriver;
using Vrumm.Application.Drivers.Commands.UpdateDriver;
using Vrumm.Application.Drivers.Commands.UploadLicense;
using Vrumm.Application.Drivers.Queries.GetDrivers;
using Vrumm.Application.Motorcycles.Commands.CreateMotorcycle;
using Vrumm.Application.Motorcycles.Commands.DeleteMotorcycle;
using Vrumm.Application.Motorcycles.Commands.UpdateMotorcycle;
using Vrumm.Application.Motorcycles.Events;
using Vrumm.Application.Motorcycles.Policies;
using Vrumm.Application.Motorcycles.Queries.GetMotorcycles;
using Vrumm.Application.Plans;
using Vrumm.Application.Plans.Queries.GetPlans;
using Vrumm.Application.Rentals.Commands.CancelRental;
using Vrumm.Application.Rentals.Commands.CreateRental;
using Vrumm.Application.Rentals.Commands.FinalizeRental;
using Vrumm.Application.Rentals.Dtos;
using Vrumm.Application.Rentals.Events;
using Vrumm.Application.Rentals.Queries.CalculateReturnValue;
using Vrumm.Application.Rentals.Queries.GetRentals;
using Vrumm.Domain.Options;

namespace Vrumm.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommon();
        services.AddMotorcycles();
        services.AddDrivers(configuration);
        services.AddPlans(configuration);
        services.AddRentals();
        services.AddJwt(configuration);
        services.AddPerformance(configuration);

        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        services.AddScoped<ICommandBus>(provider =>
        {
            var innerBus = provider.GetRequiredService<CommandBus>();
            return new PerformanceBehavior(innerBus, provider.GetRequiredService<ILogger<PerformanceBehavior>>(),
                provider.GetRequiredService<IOptions<PerformanceOptions>>());
        });

        return services;
    }

    public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<ITokenService, Auth.Implementations.JwtTokenService>();
        return services;
    }

    public static IServiceCollection AddPerformance(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PerformanceOptions>(configuration.GetSection(PerformanceOptions.SectionName));
        return services;
    }

    public static IServiceCollection AddMotorcycles(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<DeleteMotorcycleCommand>, DeleteMotorcycleCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateMotorcycleCommand>, UpdateMotorcycleCommandHandler>();

        services.AddScoped<GetMotorcyclesQueryHandler>();

        services.AddScoped<MotorcycleRegisteredNotificationHandler>();
        services.AddSingleton<IMotorcycleRegistrationPolicy, MotorcycleRegistrationPolicy>();

        services.AddScoped<CreateMotorcycleCommandHandler>();
        services.AddScoped<UpdateMotorcycleCommandHandler>();
        services.AddScoped<DeleteMotorcycleCommandHandler>();

        services.AddScoped<IValidator<CreateMotorcycleCommand>, CreateMotorcycleCommandValidator>();
        services.AddScoped<IValidator<UpdateMotorcycleCommand>, UpdateMotorcycleCommandValidator>();
        services.AddScoped<IValidator<DeleteMotorcycleCommand>, DeleteMotorcycleCommandValidator>();

        return services;
    }

    public static IServiceCollection AddDrivers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICommandHandler<CreateDriverCommand, Guid>, CreateDriverCommandHandler>();
        services.AddScoped<ICommandHandler<UploadLicenseCommand, string>, UploadLicenseCommandHandler>();

        services.AddScoped<GetDriversQueryHandler>();

        services.AddScoped<CreateDriverCommandHandler>();
        services.AddScoped<UpdateDriverCommandHandler>();
        services.AddScoped<DeleteDriverCommandHandler>();
        services.AddScoped<UploadLicenseCommandHandler>();

        services.AddScoped<IValidator<CreateDriverCommand>, CreateDriverCommandValidator>();
        services.AddScoped<IValidator<UpdateDriverCommand>, UpdateDriverCommandValidator>();
        services.AddScoped<IValidator<DeleteDriverCommand>, DeleteDriverCommandValidator>();
        services.AddScoped<IValidator<UploadLicenseCommand>, UploadLicenseCommandValidator>();

        return services;
    }

    public static IServiceCollection AddRentals(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateRentalCommand, Guid>, CreateRentalCommandHandler>();
        services.AddScoped<ICommandHandler<FinalizeRentalCommand>, FinalizeRentalCommandHandler>();
        services.AddScoped<IQueryHandler<GetRentalsQuery, PaginatedList<RentalDto>>, GetRentalsQueryHandler>();
        services.AddScoped<ICommandHandler<CancelRentalCommand>, CancelRentalCommandHandler>();

        services.AddScoped<GetRentalsQueryHandler>();
        services.AddScoped<CalculateReturnValueQueryHandler>();

        services.AddScoped<RentalCreatedNotificationHandler>();
        services.AddScoped<RentalFinalizedNotificationHandler>();

        services.AddScoped<CreateRentalCommandHandler>();
        services.AddScoped<FinalizeRentalCommandHandler>();
        services.AddScoped<CancelRentalCommandHandler>();

        services.AddScoped<IValidator<CreateRentalCommand>, CreateRentalCommandValidator>();
        services.AddScoped<IValidator<FinalizeRentalCommand>, FinalizeRentalCommandValidator>();
        services.AddScoped<IValidator<CancelRentalCommand>, CancelRentalCommandValidator>();

        return services;
    }

    public static IServiceCollection AddPlans(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PlanOptions>(configuration.GetSection(PlanOptions.SectionName));
        services.AddSingleton<IPlanFactory, PlanFactory>();
        services.AddScoped<GetPlansQueryHandler>();
        return services;
    }

    public static IServiceCollection AddCommon(this IServiceCollection services)
    {
        services.AddScoped<Dispatcher>();
        return services;
    }
}