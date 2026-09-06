using Microsoft.EntityFrameworkCore;
using ProductManager.DAL;
using ProductManager.BAL.Services; 
using ProductManager.BAL.DTO;
using ProductManager.DAL.Models; 
using ProductManager.BAL.Tests.Interceptors;  

namespace ProductManager.BAL.Tests.ProductServiceTests;


public class CrearteAsyncTests
{
    private static ProductManagerDBContext CreateContext()
    {  
        var options = new DbContextOptionsBuilder<ProductManagerDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new ConcurrencyTokenInterceptor(nameof(Product.ConcurrencyToken)))
            .AddInterceptors(new UniqueConstraintInterceptor<Product>(nameof(Product.Name)))
            .Options;
        return new ProductManagerDBContext(options);
    }

    [Theory]
    [InlineData(true, "PRD1")]
    [InlineData(false, "PRD1", "PRD1")]
    [InlineData(true, "PRD1", "PRD2")]
    [InlineData(false, "PRD1", "PRD2", "PRD1")]
    public async Task CreateAsync_CreatesProductsWithExpectedSuccess(bool expectSuccess, params string[] names )
    {
        if (names.Length == 0)
        {
            throw new Exception("Invalid test-fill the names params");
        }

        await using var ctx =  CreateContext(); 

        ProductService service = new(ctx);
        int fixedQuantity = 2;
        bool hadException = false; 
        for (int i=0; !hadException && i<names.Length; i++)
        {
            string name = names[i];
            Exception? exception = await Record.ExceptionAsync(async () => await service.CreateAsync(
            new ProductDTO {Id=i+1,Name = name, Quantity = fixedQuantity },
            CancellationToken.None));
            hadException = exception is not null;
        }

        Assert.Equal(expectSuccess, !hadException);
    }

    [Fact]
    public async Task CreateAsync_NegativeQuantityValueShouldBeSavedAsZero() //Maybe we need to fix something in the service
    {
        await using var ctx = CreateContext(); 
        ProductDTO newPrd = new ProductDTO
        {
            Name = "PRD1",
            Quantity = -3
        };

        ProductService service = new(ctx);
        await service.CreateAsync(newPrd, CancellationToken.None);
        Product? prd = await ctx.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == 1);

        Assert.NotNull(prd);
        if (prd is not null)
        {
            Assert.Equal(0, prd.Quantity);
        }
    }
}