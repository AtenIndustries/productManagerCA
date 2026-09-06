using Microsoft.EntityFrameworkCore;
using ProductManager.DAL;
using ProductManager.DAL.Models;
using ProductManager.BAL.Tests.Interceptors;

namespace ProductManager.BAL.Tests.ProductServiceTests;

public static class ContextGenerators
{
    public static ProductManagerDBContext CreateSimpleContext()
    {
        var options = new DbContextOptionsBuilder<ProductManagerDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ProductManagerDBContext(options);
    }
    public static ProductManagerDBContext CreateContextWithForcedException<T>() where T : Exception, new()
    {
        var options = new DbContextOptionsBuilder<ProductManagerDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new ForceExceptionInterceptor<T>())
            .Options;
        return new ProductManagerDBContext(options);
    }
    public static ProductManagerDBContext CreateCtxWConcurrencyTknAndUnkCnstrntInterceptor()
    {
        var options = new DbContextOptionsBuilder<ProductManagerDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new ConcurrencyTokenInterceptor(nameof(Product.ConcurrencyToken)))
            .AddInterceptors(new UniqueConstraintInterceptor<Product>(nameof(Product.Name)))
            .Options;
        return new ProductManagerDBContext(options);
    }

    public static async Task<ProductManagerDBContext> CreateSimpleContextWithData()
    {
        var options = new DbContextOptionsBuilder<ProductManagerDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new ProductManagerDBContext(options);

        ctx.Products.Add(new Product { Id = 1, Name = "PRD1", Quantity = 1, ConcurrencyToken = [1, 1, 1, 1] });
        ctx.Products.Add(new Product { Id = 2, Name = "PRD2", Quantity = 4, ConcurrencyToken = [2, 2, 2, 2] });
        ctx.Products.Add(new Product { Id = 3, Name = "PRD3", Quantity = 5, ConcurrencyToken = [3, 3, 3, 3] });
        ctx.Products.Add(new Product { Id = 4, Name = "PRD4", Quantity = 8, ConcurrencyToken = [4, 4, 4, 4] });
        ctx.Products.Add(new Product { Id = 5, Name = "CHEM1", Quantity = 9, ConcurrencyToken = [5, 5, 5, 5] });
        ctx.Products.Add(new Product { Id = 6, Name = "CHEM2", Quantity = 7, ConcurrencyToken = [6, 6, 6, 6] });

        await ctx.SaveChangesAsync();
        return ctx;
    }
}