using Io.Cucumber.Messages.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductManager.API.Tests.Interceptors;
using ProductManager.DAL;

namespace ProductManager.API.Tests.Support;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    //Generates one unique db name per run, or we end up creating multiple memory dbs
    //When running BDD tests
    private readonly string _dbName = $"ApiTestDb_{Guid.NewGuid()}";  
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            /*var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ProductManagerDBContext>));
            if (descriptor != null) services.Remove(descriptor);*/
            //To avoid conflicts with EFCore In memory db 
            services.RemoveAll<DbContextOptions<ProductManagerDBContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ProductManagerDBContext>>();

            services.AddDbContext<ProductManagerDBContext>(options =>
                options.UseInMemoryDatabase(_dbName).AddInterceptors(new ConcurrencyTokenInterceptor(nameof(DAL.Models.Product.ConcurrencyToken))));
        });
    }
}