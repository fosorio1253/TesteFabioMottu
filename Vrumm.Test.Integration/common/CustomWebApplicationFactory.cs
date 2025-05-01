using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Vrumm.Infrastructure.Data.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Vrumm.Test.Integration.common;
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(static services =>
        {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<VrummDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<VrummDbContext>(options =>
                options.UseInMemoryDatabase("VrummTestDb"));

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VrummDbContext>();
            db.Database.EnsureCreated();

            SeedData.Initialize(db);
        });
    }
}