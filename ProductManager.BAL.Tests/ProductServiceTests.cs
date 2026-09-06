using Microsoft.EntityFrameworkCore;
using ProductManager.DAL; 
using ProductManager.BAL.Services;
using ProductManager.BAL.Exceptions;
using ProductManager.BAL.DTO;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.Tests;


public class ProductServiceTests
{
    private static ProductManagerDBContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ProductManagerDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // BD nova e isolada por teste
            .Options;
        return new ProductManagerDBContext(options);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsProductNotFoundException_OnNonExistingProduct()
    {
        await using var ctx = CreateContext();
        ProductService service = new(ctx);
        await Assert.ThrowsAsync<ProductNotFoundException>(()=>service.UpdateAsync(999, new ProductDTO(), CancellationToken.None));
    }

    [Fact]
    public async Task DecrementStock_CanResultInNegative() //Maybe we need to fix something in the service
    {
        
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
