using Microsoft.EntityFrameworkCore;
using ProductManager.DAL;
using ProductManager.BAL.Services;
using ProductManager.BAL.DTO;
using ProductManager.DAL.Models;
using ProductManager.BAL.Exceptions;

namespace ProductManager.BAL.Tests.ProductServiceTests;


public class CreateAsyncTests : ProductServiceTests
{
    [Theory]
    [InlineData(true, "PRD1")]
    [InlineData(false, "PRD1", "PRD1")]
    [InlineData(true, "PRD1", "PRD2")]
    [InlineData(false, "PRD1", "PRD2", "PRD1")]
    public async Task CreateAsync_UniqueOrDuplicateNames_SucceedsOrThrowsOnDuplicate(bool expectSuccess, params string[] names)
    {
        if (names.Length == 0)
        {
            throw new Exception("Invalid test-fill the names params");
        }

        await using var ctx = ContextGenerators.CreateCtxWConcurrencyTknAndUnkCnstrntInterceptor();

        ProductService service = CreateProductService(ctx);
        int fixedQuantity = 2;
        bool hadException = false;
        for (int i = 0; !hadException && i < names.Length; i++)
        {
            string name = names[i];
            Exception? exception = await Record.ExceptionAsync(async () => await service.CreateAsync(
            new ProductDTO { Name = name, Quantity = fixedQuantity },
            CancellationToken.None));
            hadException = exception is not null;
        }

        Assert.Equal(expectSuccess, !hadException);
    }

    [Fact]
    public async Task CreateAsync_NegativeQuantity_IsSetToZero()
    {
        await using var ctx = ContextGenerators.CreateCtxWConcurrencyTknAndUnkCnstrntInterceptor();
        ProductDTO newPrd = new()
        {
            Name = "PRD1",
            Quantity = -3
        };

        ProductService service = CreateProductService(ctx);
        await service.CreateAsync(newPrd, CancellationToken.None);
        Product? prd = await ctx.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == 1);

        Assert.NotNull(prd);
        if (prd is not null)
        {
            Assert.Equal(0, prd.Quantity);
        }
    }

    [Fact]
    public async Task CreateAsync_ConcurrentModificationDetected_ThrowsProductConcurrencyException()
    {
        await using var ctx = ContextGenerators.CreateContextWithForcedException<DbUpdateConcurrencyException>();
        ProductService service = CreateProductService(ctx);
        await Assert.ThrowsAsync<ProductConcurrencyException>(() => service.CreateAsync(new ProductDTO { Quantity = 2, Name = "PRD" }));
    }
}