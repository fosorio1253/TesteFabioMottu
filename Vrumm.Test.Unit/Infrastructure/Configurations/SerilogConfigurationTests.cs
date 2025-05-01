using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.Configurations;
public class SerilogConfigurationTests
{
    /*
    [Fact]
    public void AddSerilogConfiguration_ConfiguresLogger()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "GoogleCloud:ProjectId", "test-project" }
            })
            .Build();

        // Act
        services.AddSerilogConfiguration(configuration);

        // Assert
        var provider = services.BuildServiceProvider();
        var logger = provider.GetService<ILogger<SerilogConfigurationTests>>();
        logger.Should().NotBeNull();
    }
    */
}