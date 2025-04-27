using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Vrumm.Application.Drivers.Commands.CreateDriver;
using Vrumm.Application.Drivers.Commands.DeleteDriver;
using Vrumm.Application.Drivers.Commands.UpdateDriver;
using Vrumm.Application.Drivers.Commands.UploadLicense;
using Vrumm.Application.Drivers.Queries.GetDrivers;
using Vrumm.Application.Motorcycles.Commands.CreateMotorcycle;
using Vrumm.Application.Motorcycles.Commands.DeleteMotorcycle;
using Vrumm.Application.Motorcycles.Commands.UpdateMotorcycle;
using Vrumm.Application.Motorcycles.Events;
using Vrumm.Application.Motorcycles.Queries.GetMotorcycles;
using Vrumm.Application.Plans.Queries.GetPlans;
using Vrumm.Application.Rentals.Commands.CancelRental;
using Vrumm.Application.Rentals.Commands.CreateRental;
using Vrumm.Application.Rentals.Commands.FinalizeRental;
using Vrumm.Application.Rentals.Events;
using Vrumm.Application.Rentals.Queries.CalculateReturnValue;
using Vrumm.Application.Rentals.Queries.GetRentals;

namespace Vrumm.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddCommon();
        services.AddMotorcycles();
        services.AddDrivers();
        services.AddPlans();
        services.AddRentals();

        return services;
    }
    
    public static IServiceCollection AddMotorcycles(this IServiceCollection services)
    {
        services.AddScoped<GetMotorcyclesQueryHandler>();

        services.AddScoped<MotorcycleRegisteredNotificationHandler>();

        services.AddScoped<CreateMotorcycleCommandHandler>();
        services.AddScoped<UpdateMotorcycleCommandHandler>();
        services.AddScoped<DeleteMotorcycleCommandHandler>();

        services.AddScoped<IValidator<CreateMotorcycleCommand>, CreateMotorcycleCommandValidator>();
        services.AddScoped<IValidator<UpdateMotorcycleCommand>, UpdateMotorcycleCommandValidator>();
        services.AddScoped<IValidator<DeleteMotorcycleCommand>, DeleteMotorcycleCommandValidator>();

        return services;
    }

    public static IServiceCollection AddDrivers(this IServiceCollection services)
    {
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

    public static IServiceCollection AddPlans(this IServiceCollection services)
    {
        services.AddScoped<GetPlansQueryHandler>();
        return services;
    }

    public static IServiceCollection AddCommon(this IServiceCollection services)
    {
        services.AddScoped<Dispatcher>();
        return services;
    }
}