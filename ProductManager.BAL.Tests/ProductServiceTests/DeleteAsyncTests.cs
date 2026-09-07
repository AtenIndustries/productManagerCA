using Microsoft.EntityFrameworkCore; 
using ProductManager.BAL.Services; 
using ProductManager.BAL.Exceptions;

namespace ProductManager.BAL.Tests.ProductServiceTests;

public class DeleteAsyncTests
{

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(4, false)]
    public async Task DeleteAsync_ExistingOrNonExistingId_SucceedsOrThrowsProductNotFoundException(int delId, bool expectSuccess)
    {
        await using var ctx = ContextGenerators.CreateSimpleContext();
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
    public async Task DeleteAsync_ConcurrentModificationDetected_ThrowsProductConcurrencyException()
    {
        await using var ctx = ContextGenerators.CreateContextWithForcedException<DbUpdateConcurrencyException>();
        ctx.Products.Add(new DAL.Models.Product { Id = 1, Name = "PRD1", ConcurrencyToken = [1, 1, 1, 1] });
        ctx.Products.Add(new DAL.Models.Product { Id = 2, Name = "PRD2", ConcurrencyToken = [2, 2, 2, 2] });
        ctx.Products.Add(new DAL.Models.Product { Id = 3, Name = "PRD3", ConcurrencyToken = [3, 3, 3, 3] });
        // Using a sync call on purpose. The Inteceptor will only work on async calls
        // Allows the insertion of some data to prevent ProductNotFoundExceptions 
        ctx.SaveChanges();

        ProductService service = new(ctx);
        await Assert.ThrowsAsync<ProductConcurrencyException>(() => service.DeleteAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_UnexpectedDbUpdateException_ThrowsProductPersistenceException()
    {
        await using var ctx = ContextGenerators.CreateContextWithForcedException<DbUpdateException>();
        ctx.Products.Add(new DAL.Models.Product { Id = 1, Name = "PRD1", ConcurrencyToken = [1, 1, 1, 1] });
        ctx.Products.Add(new DAL.Models.Product { Id = 2, Name = "PRD2", ConcurrencyToken = [2, 2, 2, 2] });
        ctx.Products.Add(new DAL.Models.Product { Id = 3, Name = "PRD3", ConcurrencyToken = [3, 3, 3, 3] });
        //This is sync in purpose. The Inteceptor will only work on async calls and need to insert
        //some data before changes anything is deleted, to prevent ProductNotFoundExceptions
        ctx.SaveChanges();

        ProductService service = new(ctx);
        await Assert.ThrowsAsync<ProductPersistenceException>(() => service.DeleteAsync(1));
    }
}