using Microsoft.EntityFrameworkCore;
using ProductManager.DAL; 
using ProductManager.BAL.Services;
using ProductManager.BAL.Exceptions;
using ProductManager.BAL.DTO;
using ProductManager.DAL.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace ProductManager.BAL.Tests.ProductServiceTests;


public class ProductServiceTests
{
    private static ProductManagerDBContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ProductManagerDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // BD nova e isolada por teste
            .Options;
        return new ProductManagerDBContext(options);
    }

    [Theory]
    [InlineData(2, "PRD2-Upgraded", 4)]
    [InlineData(3, "PRD3-Downgraded", 2)]
    [InlineData(1, "PRD1-New Release", 5)]
    public async Task UpdateAsync_UpdatesProductWithSuccess(int updId, string newName, int newQuantity)
    {
        await using var ctx = CreateContext();
        ctx.Products.Add(new DAL.Models.Product{Id=1, Name="PRD1", ConcurrencyToken = [1,1,1,1]});
        ctx.Products.Add(new DAL.Models.Product{Id=2, Name="PRD2", ConcurrencyToken = [2,2,2,2]});
        ctx.Products.Add(new DAL.Models.Product{Id=3, Name="PRD3", ConcurrencyToken = [3,3,3,3]});
        await ctx.SaveChangesAsync(); 


        ProductDTO updPrd = new()
        {
            Name=newName,
            Quantity=newQuantity
        };

        ProductService service = new(ctx);

        int id = await service.UpdateAsync(updId, updPrd, CancellationToken.None);
        Assert.Equal(updId, id);

        Product? updEntity = await ctx.Products.AsNoTracking().FirstOrDefaultAsync(p=>p.Id == updId);

        Assert.NotNull(updEntity);
        Assert.Equal(newName, updEntity.Name);
        Assert.Equal(newQuantity, updEntity.Quantity);

    }


    [Fact]
    public async Task UpdateAsync_ThrowsProductNotFoundException_OnNonExistingProduct()
    {
        await using var ctx = CreateContext();
        ProductService service = new(ctx);
        await Assert.ThrowsAsync<ProductNotFoundException>(()=>service.UpdateAsync(999, new ProductDTO(), CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_NegativeQuantityValueShouldBeSavedAsZero() //Maybe we need to fix something in the service
    {
        await using var ctx = CreateContext();
        ctx.Products.Add(new DAL.Models.Product{Id=1, Name="PRD1", Quantity=2, ConcurrencyToken = [1,1,1,1]});
        await ctx.SaveChangesAsync(); 
        ctx.ChangeTracker.Clear();

        ProductDTO updPrd = new ProductDTO
        {
            Name="PRD1",
            Quantity=-3
        };

        ProductService service = new(ctx);
        await service.UpdateAsync(1, updPrd, CancellationToken.None);
        Product? prd = await ctx.Products.AsNoTracking().FirstOrDefaultAsync(p=>p.Id==1);

        Assert.NotNull(prd);
        if (prd is not null)
        {
            Assert.Equal(0, prd.Quantity);
        }
    }

    [Fact]
    public async Task UpdateAsync_ThrowsDuplicateProductException_WhenTryingToUpdateNameToAnExistingOne()
    {
        await using var ctx = CreateContext();
        ctx.Products.Add(new DAL.Models.Product{Id=1, Name="PRD1", ConcurrencyToken = [1,1,1,1]});
        ctx.Products.Add(new DAL.Models.Product{Id=2, Name="PRD2", ConcurrencyToken = [2,2,2,2]});
        ctx.Products.Add(new DAL.Models.Product{Id=3, Name="PRD3", ConcurrencyToken = [3,3,3,3]});
        await ctx.SaveChangesAsync(); 
        ctx.ChangeTracker.Clear();

        ProductDTO updPrd = new ProductDTO
        {
            Name="PRD2",
            Quantity=2
        };

        ProductService service = new(ctx);

        await Assert.ThrowsAsync<DuplicateProductException>(
            ()=> service.UpdateAsync(3, updPrd, CancellationToken.None)
        );
    }

    [Fact]
    public async Task UpdateAsync_ThrowsProductConflictException_OnConcurrencyConflict()
    {
        await using var ctx = CreateContext();
        var product = new Product {Id = 1, Name = "PRD1", Quantity=3, ConcurrencyToken = new byte[]{1,1,1,1}};
        ctx.Products.Add(product);
        await ctx.SaveChangesAsync();
        ctx.ChangeTracker.Clear();

        var service = new ProductService(ctx);

        // Simulates that another entry already saved a different version
        var entity = await ctx.Products.FirstAsync(p=>p.Id==1);
        ctx.Entry(entity).OriginalValues[nameof(Product.ConcurrencyToken)] = new byte[]{2,2,2,2};

        var updateDto = new ProductDTO{Name="PRD1-Upd", Quantity=5};

        await Assert.ThrowsAsync<ProductConcurrencyException>(()=>service.UpdateAsync(1, updateDto, CancellationToken.None));
    }

}
