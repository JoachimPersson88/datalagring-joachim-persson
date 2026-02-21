using EducationForEvery1.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;

namespace EducationForEvery1.Tests;
// TestApplication är en anpassad WebApplicationFactory som används för integrationstester.
public class TestApplication : WebApplicationFactory<Program>
{
    // ConfigureWebHost är en metod som låter oss anpassa hur webbappen startas upp under testerna.
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Vi sätter miljön till "Testing" så att vi kan ha olika konfigurationer för testning.
        builder.UseEnvironment("Testing");
        // Här konfigurerar vi tjänsterna som används av webbappen.
        builder.ConfigureServices(services =>
        {
            // Vi letar upp alla tjänster som är relaterade till EducationDbContext och tar bort dem.
            var descriptors = services.Where(d =>
                // Vi kollar både ServiceType och ImplementationType för att vara säkra på att vi tar bort alla relevanta tjänster.
                d.ServiceType == typeof(DbContextOptions<EducationDbContext>) ||
                // Vi kollar både ServiceType och ImplementationType för att vara säkra på att vi tar bort alla relevanta tjänster.
                d.ServiceType == typeof(EducationDbContext) ||
                // Vi kollar både ServiceType och ImplementationType för att vara säkra på att vi tar bort alla relevanta tjänster.
                d.ImplementationType == typeof(EducationDbContext)).ToList();

            // Vi tar bort alla tjänster som matchar våra kriterier så att vi kan ersätta dem med en in-memory databas.
            foreach (var d in descriptors)
            {
                // Vi tar bort tjänsten från service collection så att den inte längre används i testerna.
                services.Remove(d);
            }
            // Vi lägger till en ny DbContext som använder en in-memory databas istället för den riktiga databasen.
            services.AddDbContext<EducationDbContext>(options =>
                // Vi konfigurerar DbContext att använda en in-memory databas med namnet "TestDb".
                options.UseInMemoryDatabase("TestDb"));
        });
    }
}