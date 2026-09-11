using Microsoft.EntityFrameworkCore;
using ProductManager.BAL.Services;
using ProductManager.BAL.Exceptions;
using ProductManager.BAL.DTO;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.Tests.ProductServiceTests;


public class UpdateAsyncTests : ProductSearchTests
{
    [Theory]
    [InlineData(2, "PRD2-Upgraded", "PRD2-Upgraded is an upgraded version of PRD2", 4)]
    [InlineData(3, "PRD3-Downgraded", "PRD3-Downgraded is a lightweight version of PRD3", 2)]
    [InlineData(1, "PRD1-New Release", null, 5)]
    public async Task UpdateAsync_ValidParams_UpdatesProductSuccessfully(int updId, string newName, string? newDescription, int newQuantity)
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ProductDataDTO updPrd = new()
        {
            Name = newName,
            Quantity = newQuantity,
            Description = newDescription
        };

        ProductService service = CreateProductService(ctx);

        ProductDTO prd = await service.UpdateAsync(updId, updPrd, CancellationToken.None);
        Assert.Equal(updId, prd.Id);

        Product? updEntity = await ctx.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == updId);

        Assert.NotNull(updEntity);
        Assert.Equal(newName, updEntity.Name);
        Assert.Equal(newQuantity, updEntity.Quantity);

    }

    [Theory]
    [InlineData(2, 4)]
    [InlineData(3, 2)]
    [InlineData(1, 5)]
    public async Task UpdateAsync_SameNameSameIdDifferentQuantity_DoesNotThrowDuplicateProductException(int updId, int newQuantity)
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();

        Product? entity = await ctx.Products.FirstOrDefaultAsync(p => p.Id == updId);
        Assert.NotNull(entity);
        ctx.ChangeTracker.Clear();

        ProductDataDTO updPrd = new()
        {
            Quantity = newQuantity,
            Name = entity.Name,
            Description = entity.Description
        };

        ProductService service = CreateProductService(ctx);
        Exception? exception = await Record.ExceptionAsync(async () => await service.UpdateAsync(updId, updPrd, CancellationToken.None));
        Assert.False(exception is DuplicateProductException);
    }


    [Theory]
    [InlineData(990)]
    [InlineData(12)]
    [InlineData(15)]
    public async Task UpdateAsync_NonExistingProduct_ThrowsProductNotFoundException(int updId)
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ctx.ChangeTracker.Clear();

        ProductService service = CreateProductService(ctx);
        await Assert.ThrowsAsync<ProductNotFoundException>(() => service.UpdateAsync(updId, new ProductDataDTO(), CancellationToken.None));
    }

    [Theory]
    [InlineData(3, "PRD2", 4)]
    [InlineData(2, "PRD1", 2)]
    [InlineData(1, "PRD3", 5)]
    public async Task UpdateAsync_NameAlreadyUsedByAnotherProduct_ThrowsDuplicateProductException(int updId, string updName, int updQuantity)
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ctx.ChangeTracker.Clear();

        ProductDataDTO updPrd = new()
        {
            Name = updName,
            Quantity = updQuantity
        };

        ProductService service = CreateProductService(ctx);

        await Assert.ThrowsAsync<DuplicateProductException>(
            () => service.UpdateAsync(updId, updPrd, CancellationToken.None)
        );
    }


    [Fact]
    public async Task UpdateAsync_NegativeQuantity_IsSetToZero()
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ctx.ChangeTracker.Clear();

        ProductDataDTO updPrd = new()
        {
            Name = "PRD1",
            Quantity = -3
        };

        ProductService service = CreateProductService(ctx);
        await service.UpdateAsync(1, updPrd, CancellationToken.None);
        Product? prd = await ctx.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == 1);

        Assert.NotNull(prd);
        if (prd is not null)
        {
            Assert.Equal(0, prd.Quantity);
        }
    }


    [Fact]
    public async Task UpdateAsync_ConcurrentModificationDetected_ThrowsProductConflictException()
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ctx.ChangeTracker.Clear();

        ProductService service = CreateProductService(ctx);

        // Simulates that another entry already saved a different version
        var entity = await ctx.Products.FirstAsync(p => p.Id == 1);
        ctx.Entry(entity).OriginalValues[nameof(Product.ConcurrencyToken)] = new byte[] { 2, 2, 2, 2 };

        ProductDataDTO updateDto = new() { Name = "PRD1-Upd", Quantity = 5 };

        await Assert.ThrowsAsync<ProductConcurrencyException>(() => service.UpdateAsync(1, updateDto, CancellationToken.None));
    }
}
