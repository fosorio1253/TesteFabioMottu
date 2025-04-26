using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.GoogleCloudLogging;

namespace Vrumm.Infrastructure;
public static class SerilogConfiguration
{
    public static IHostBuilder UseVrummSerilog(this IHostBuilder hostBuilder,
        Action<LoggingProviderOptions> configureOptions = null)
    {
        var options = new LoggingProviderOptions();

        configureOptions?.Invoke(options);

        return hostBuilder.UseSerilog((context, services, configuration) =>
        {
            var env = context.HostingEnvironment;
            var appName = env.ApplicationName;

            configuration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Application", appName)
                .Enrich.WithProperty("Environment", env.EnvironmentName)
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information);

            configuration.WriteTo.Console(
                outputTemplate: options.Console.OutputTemplate);

            ConfigureLogProviders(configuration, options, env.IsDevelopment());

            configuration.ReadFrom.Configuration(context.Configuration);
        });
    }

    private static void ConfigureLogProviders(LoggerConfiguration config, LoggingProviderOptions options, bool isDevelopment)
    {
        if (isDevelopment) return;

        var enabledProviders = new List<LogProvider>();

        if (enabledProviders.Contains(LogProvider.GoogleCloud) &&
            !string.IsNullOrEmpty(options.GoogleCloud.ProjectId))
        {
            config.WriteTo.GoogleCloudLogging(
                new GoogleCloudLoggingSinkOptions
                {
                    ProjectId = options.GoogleCloud.ProjectId
                });
        }
    }

    public enum LogProvider
    {
        GoogleCloud
    }

    public class LoggingProviderOptions
    {
        public ConsoleLogOptions Console { get; set; } = new ConsoleLogOptions();
        public GoogleCloudLogOptions GoogleCloud { get; set; } = new GoogleCloudLogOptions();

        public class ConsoleLogOptions
        {
            public string OutputTemplate { get; set; } = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
        }

        public class GoogleCloudLogOptions
        {
            public string ProjectId { get; set; }
        }
    }
}