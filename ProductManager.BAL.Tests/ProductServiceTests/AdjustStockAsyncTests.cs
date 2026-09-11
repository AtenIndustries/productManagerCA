using Microsoft.EntityFrameworkCore;
using ProductManager.BAL.Services;
using ProductManager.BAL.Exceptions;
using ProductManager.BAL.DTO;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.Tests.ProductServiceTests;


public class AdjustStockAsyncTests : ProductServiceTests
{
    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, -9)]
    [InlineData(3, 1)]
    [InlineData(4, -1)]
    public async Task AdjustStockAsync_UpdatesProductQuantitySuccessfully_OnValidParams(int id, int delta)
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ProductService service = CreateProductService(ctx);

        //Gets original product data to determine if stock was updated
        ProductReadDTO? prd = await service.GetAsync(id, CancellationToken.None);
        ProductReadDTO adjPrd = await service.AdjustStockAsync(id, delta, CancellationToken.None);
        Assert.NotNull(prd);
        Assert.Equal(id, adjPrd.Id);
        Assert.Equal(Math.Max(prd.Quantity + delta, 0), adjPrd.Quantity);
    }

    [Fact]
    public async Task AdjustStockAsync_ThrowsProductNotFoundException_WhenUpdatingNonExisting()
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ProductService service = CreateProductService(ctx);
        await Assert.ThrowsAsync<ProductNotFoundException>(() => service.AdjustStockAsync(999, 500, CancellationToken.None));
    }

    [Fact]
    public async Task AdjustStockAsync_ThrowsProductConcurrencyException_WhenDbUpdateConcurrencyExceptionOccurs()
    {
        await using var ctx = ContextGenerators.CreateContextWithForcedException<DbUpdateConcurrencyException>();
        //Adds a product sync just to pass the search
        ctx.Products.Add(new Product() { Id = 999, Name = "PRD", ConcurrencyToken = [1, 2, 3, 4] });
        ctx.SaveChanges();
        ProductService service = CreateProductService(ctx);
        await Assert.ThrowsAsync<ProductConcurrencyException>(() => service.AdjustStockAsync(999, 500, CancellationToken.None));
    }
}