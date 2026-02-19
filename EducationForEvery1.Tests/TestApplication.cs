using EducationForEvery1.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace EducationForEvery1.Tests;

public class TestApplication : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Ta bort riktig DbContext-registrering (SQL Server)
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<EducationDbContext>));

            if (descriptor is not null)
                services.Remove(descriptor);

            // Lägg till InMemory DB istället
            services.AddDbContext<EducationDbContext>(options =>
                options.UseInMemoryDatabase("TestDb"));
        });
    }
}