using Microsoft.EntityFrameworkCore;
using ProductManager.DAL;
using ProductManager.BAL.Services;
using ProductManager.BAL.Exceptions;
using ProductManager.BAL.DTO;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.Tests.ProductServiceTests;


public class UpdateAsyncTests
{
    private static ProductManagerDBContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ProductManagerDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ProductManagerDBContext(options);
    }

    [Theory]
    [InlineData(2, "PRD2-Upgraded", 4)]
    [InlineData(3, "PRD3-Downgraded", 2)]
    [InlineData(1, "PRD1-New Release", 5)]
    public async Task UpdateAsync_UpdatesProductWithSuccess(int updId, string newName, int newQuantity)
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ProductDTO updPrd = new()
        {
            Name = newName,
            Quantity = newQuantity
        };

        ProductService service = new(ctx);

        int id = await service.UpdateAsync(updId, updPrd, CancellationToken.None);
        Assert.Equal(updId, id);

        Product? updEntity = await ctx.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == updId);

        Assert.NotNull(updEntity);
        Assert.Equal(newName, updEntity.Name);
        Assert.Equal(newQuantity, updEntity.Quantity);

    }

    [Theory]
    [InlineData(2, 4)]
    [InlineData(3, 2)]
    [InlineData(1, 5)]
    public async Task UpdateAsync_UpdateQuantityButKeepingNameDoesNotThrowDuplicateException(int updId, int newQuantity)
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();

        Product? entity = await ctx.Products.FirstOrDefaultAsync(p => p.Id == updId);
        Assert.NotNull(entity);
        ctx.ChangeTracker.Clear();

        ProductDTO updPrd = ProductDTO.FromEntity(entity);
        updPrd.Quantity = newQuantity;

        ProductService service = new(ctx);
        Exception? exception = await Record.ExceptionAsync(async () => await service.UpdateAsync(updId, updPrd, CancellationToken.None));
        Assert.False(exception is DuplicateProductException);
    }


    [Theory]
    [InlineData(990)]
    [InlineData(12)]
    [InlineData(15)]
    public async Task UpdateAsync_ThrowsProductNotFoundException_OnNonExistingProduct(int updId)
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ctx.ChangeTracker.Clear();

        ProductService service = new(ctx);
        await Assert.ThrowsAsync<ProductNotFoundException>(() => service.UpdateAsync(updId, new ProductDTO(), CancellationToken.None));
    }

    [Theory]
    [InlineData(3, "PRD2", 4)]
    [InlineData(2, "PRD1", 2)]
    [InlineData(1, "PRD3", 5)]
    public async Task UpdateAsync_ThrowsDuplicateProductException_WhenTryingToUpdateNameToAnExistingOne(int updId, string updName, int updQuantity)
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ctx.ChangeTracker.Clear();

        ProductDTO updPrd = new ProductDTO
        {
            Name = updName,
            Quantity = updQuantity
        };

        ProductService service = new(ctx);

        await Assert.ThrowsAsync<DuplicateProductException>(
            () => service.UpdateAsync(updId, updPrd, CancellationToken.None)
        );
    }


    [Fact]
    public async Task UpdateAsync_NegativeQuantityValueShouldBeSavedAsZero()
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ctx.ChangeTracker.Clear();

        ProductDTO updPrd = new ProductDTO
        {
            Name = "PRD1",
            Quantity = -3
        };

        ProductService service = new(ctx);
        await service.UpdateAsync(1, updPrd, CancellationToken.None);
        Product? prd = await ctx.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == 1);

        Assert.NotNull(prd);
        if (prd is not null)
        {
            Assert.Equal(0, prd.Quantity);
        }
    }


    [Fact]
    public async Task UpdateAsync_ThrowsProductConflictException_OnConcurrencyConflict()
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ctx.ChangeTracker.Clear();

        ProductService service = new(ctx);

        // Simulates that another entry already saved a different version
        var entity = await ctx.Products.FirstAsync(p => p.Id == 1);
        ctx.Entry(entity).OriginalValues[nameof(Product.ConcurrencyToken)] = new byte[] { 2, 2, 2, 2 };

        ProductDTO updateDto = new() { Name = "PRD1-Upd", Quantity = 5 };

        await Assert.ThrowsAsync<ProductConcurrencyException>(() => service.UpdateAsync(1, updateDto, CancellationToken.None));
    }
}
