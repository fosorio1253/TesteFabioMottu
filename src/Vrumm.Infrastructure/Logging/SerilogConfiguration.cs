using Microsoft.Extensions.Hosting;

namespace Vrumm.Infrastructure.Logging;
public static class SerilogConfiguration
{
    public static IHostBuilder UseVrummSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, services, configuration) =>
        {
            var env = context.HostingEnvironment;
            var appName = env.ApplicationName;

            // Base configuration
            configuration
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Application", appName)
                .Enrich.WithProperty("Environment", env.EnvironmentName)
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information);

            // Configuration based on environment
            if (env.IsDevelopment())
            {
                // Console logging for development
                configuration.WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
            }
            else
            {
                // Google Cloud Logging for production
                var projectId = context.Configuration["GoogleCloud:ProjectId"];
                if (!string.IsNullOrEmpty(projectId))
                {
                    configuration.WriteTo.GoogleCloudLogging(
                        new GoogleCloudLoggingSinkOptions
                        {
                            ProjectId = projectId,
                            ResourceType = "k8s_container",
                            Labels = new Google.Api.MonitoredResource.Types.Labels
                            {
                                    { "environment", env.EnvironmentName }
                            },
                            UseJsonOutput = true
                        });
                }

                // Always add console logging for containers
                configuration.WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
            }

            // Read from configuration
            configuration.ReadFrom.Configuration(context.Configuration);
        });
    }
}