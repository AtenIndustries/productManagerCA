using Microsoft.EntityFrameworkCore;
using ProductManager.DAL;
using ProductManager.BAL.Services; 
using ProductManager.BAL.Tests.Interceptors;
using ProductManager.BAL.Exceptions;

namespace ProductManager.BAL.Tests.ProductServiceTests;

public class DeleteAsyncTests
{
    private static ProductManagerDBContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ProductManagerDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ProductManagerDBContext(options);
    }
    private static ProductManagerDBContext CreateContextWithForcedException<T>() where T: Exception, new()
    {
        var options = new DbContextOptionsBuilder<ProductManagerDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new ForceExceptionInterceptor<T>())
            .Options;
        return new ProductManagerDBContext(options);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(4, false)]
    public async Task DeleteAsync_UpdatesProductWithSuccess(int delId, bool expectSuccess)
    {
        await using var ctx = CreateContext();
        ctx.Products.Add(new DAL.Models.Product { Id = 1, Name = "PRD1", ConcurrencyToken = [1, 1, 1, 1] });
        ctx.Products.Add(new DAL.Models.Product { Id = 2, Name = "PRD2", ConcurrencyToken = [2, 2, 2, 2] });
        ctx.Products.Add(new DAL.Models.Product { Id = 3, Name = "PRD3", ConcurrencyToken = [3, 3, 3, 3] });
        await ctx.SaveChangesAsync();

        ProductService service = new(ctx);
        Exception? exception = await Record.ExceptionAsync(async () => await service.DeleteAsync(delId, CancellationToken.None));

        if (exception is not null && exception is not ProductNotFoundException)
        {
            throw exception;
        }
        bool success = exception is null;
        Assert.Equal(expectSuccess, success);
    }

    [Fact]
    public async Task DeleteAsync_ExpectProductConcurrencyException_OnDbUpdateConcurrencyException()
    {
        await using var ctx = CreateContextWithForcedException<DbUpdateConcurrencyException>(); 
        ctx.Products.Add(new DAL.Models.Product { Id = 1, Name = "PRD1", ConcurrencyToken = [1, 1, 1, 1] });
        ctx.Products.Add(new DAL.Models.Product { Id = 2, Name = "PRD2", ConcurrencyToken = [2, 2, 2, 2] });
        ctx.Products.Add(new DAL.Models.Product { Id = 3, Name = "PRD3", ConcurrencyToken = [3, 3, 3, 3] });
        //This is sync in purpose. The Inteceptor will only work on async calls and need to insert
        //some data before changes anything is deleted, to prevent ProductNotFoundExceptions
        ctx.SaveChanges(); 

        ProductService service = new (ctx);
        await Assert.ThrowsAsync<ProductConcurrencyException>(()=>service.DeleteAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_ExpectProductPersistenceException_OnDbUpdateException()
    {
        await using var ctx = CreateContextWithForcedException<DbUpdateException>(); 
        ctx.Products.Add(new DAL.Models.Product { Id = 1, Name = "PRD1", ConcurrencyToken = [1, 1, 1, 1] });
        ctx.Products.Add(new DAL.Models.Product { Id = 2, Name = "PRD2", ConcurrencyToken = [2, 2, 2, 2] });
        ctx.Products.Add(new DAL.Models.Product { Id = 3, Name = "PRD3", ConcurrencyToken = [3, 3, 3, 3] });
        //This is sync in purpose. The Inteceptor will only work on async calls and need to insert
        //some data before changes anything is deleted, to prevent ProductNotFoundExceptions
        ctx.SaveChanges(); 

        ProductService service = new (ctx);
        await Assert.ThrowsAsync<ProductPersistenceException>(()=>service.DeleteAsync(1));
    }
}