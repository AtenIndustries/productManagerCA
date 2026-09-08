using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions; 
using ProductManager.BAL.Exceptions;
using ProductManager.CommonLib.Interceptors;
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
            services.RemoveAll<DbContextOptions<ProductManagerDBContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ProductManagerDBContext>>();

            services.AddDbContext<ProductManagerDBContext>(options =>
                options.UseInMemoryDatabase(_dbName)
                .AddInterceptors(new ConcurrencyTokenInterceptor<DAL.Models.Product>(nameof(DAL.Models.Product.ConcurrencyToken)))
                .AddInterceptors(new UniqueConstraintInterceptor<DAL.Models.Product, DuplicateProductException>(nameof(DAL.Models.Product.Id), nameof(DAL.Models.Product.Name))));
        });
    }
}