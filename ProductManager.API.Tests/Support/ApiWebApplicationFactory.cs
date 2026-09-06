using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.DAL;

namespace ProductManager.API.Tests.Support;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ProductManagerDBContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<ProductManagerDBContext>(options =>
                options.UseInMemoryDatabase($"ApiTestDb_{Guid.NewGuid()}"));
        });
    }
}