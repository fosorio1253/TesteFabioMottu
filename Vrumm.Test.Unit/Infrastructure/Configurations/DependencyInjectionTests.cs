using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vrumm.Infrastructure;
using Vrumm.Infrastructure.Data.Context;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Xunit;

namespace Vrumm.Test.Unit.Infrastructure.Configurations;
public class DependencyInjectionTests
{
    [Fact]
    public void AddInfrastructure_RegistersAllServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddInfrastructure();

        // Assert
        var provider = services.BuildServiceProvider();
        provider.GetService<IUnitOfWork>().Should().NotBeNull();
        provider.GetService<VrummDbContext>().Should().NotBeNull();
    }
}