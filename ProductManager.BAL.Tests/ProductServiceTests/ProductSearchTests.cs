using Microsoft.EntityFrameworkCore;
using ProductManager.DAL;
using ProductManager.BAL.Services; 
using ProductManager.BAL.DTO; 

namespace ProductManager.BAL.Tests.ProductServiceTests;

public class ProductSearchTests
{
    private static async Task<ProductManagerDBContext> CreateSearchContext()
    {
        var options = new DbContextOptionsBuilder<ProductManagerDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // BD nova e isolada por teste
            .Options;
        var ctx= new ProductManagerDBContext(options);
        
        ctx.Products.Add(new DAL.Models.Product { Id = 1, Name = "PRD1", Quantity = 1, ConcurrencyToken = [1, 1, 1, 1] });
        ctx.Products.Add(new DAL.Models.Product { Id = 2, Name = "PRD2", Quantity = 4, ConcurrencyToken = [2, 2, 2, 2] });
        ctx.Products.Add(new DAL.Models.Product { Id = 3, Name = "PRD3", Quantity = 5, ConcurrencyToken = [3, 3, 3, 3] });
        ctx.Products.Add(new DAL.Models.Product { Id = 4, Name = "PRD4", Quantity = 8, ConcurrencyToken = [4, 4, 4, 4] });
        ctx.Products.Add(new DAL.Models.Product { Id = 5, Name = "CHEM1", Quantity = 9, ConcurrencyToken = [5, 5, 5, 5] });
        ctx.Products.Add(new DAL.Models.Product { Id = 6, Name = "CHEM2", Quantity = 7, ConcurrencyToken = [6, 6, 6, 6] });

        await ctx.SaveChangesAsync();
        return ctx;
    }

    [Theory]
    [InlineData(1, "PRD1", null, null)]
    [InlineData(2, null, 3, 6)]
    [InlineData(2, null, 4, 5)]
    [InlineData(3, null, null, 5)]
    [InlineData(2, "PRD", 4, 5)]
    [InlineData(4, "PRD",null, null)]
    [InlineData(3, "PRD",4, 8)]
    [InlineData(0, "CHEM",0, 4)]
    [InlineData(1, "CHEM2",1, null)]
    [InlineData(2, "CHEM",1, null)]
    [InlineData(6, null, null, null)]
    public async Task SearchByAsync_ReturnsTheExpectedAmountOfResults(int expectedNumberOfResults, string? name, int? min, int? max)
    {
        var ctx = await CreateSearchContext();
        ProductService service = new(ctx);
        IEnumerable<ProductDTO>? results = await service.SearchByAsync(name, min, max, CancellationToken.None);
        int numResults = results is null ? 0 : results.Count();
        Assert.Equal(expectedNumberOfResults, numResults);
    }


    [Theory]
    [InlineData(true, 1)]
    [InlineData(true, 2)]
    [InlineData(false, 7)]
    [InlineData(false, 99)]
    public async Task GetAsync_ReturnsExpectedResult(bool expectResult, int searchId)
    {
        var ctx = await CreateSearchContext();
        ProductService service = new(ctx);
        ProductDTO? prd = await service.GetAsync(searchId, CancellationToken.None);
        bool hasResult = prd is not null;
        Assert.Equal(expectResult, hasResult);
    }

    [Fact] 
    public async Task GetAllAsync_GetsAllResults()
    {
        var ctx = await CreateSearchContext();
        ProductService service = new(ctx);
        IEnumerable<ProductDTO>? results = await service.GetAllAsync( CancellationToken.None);
        Assert.NotNull(results);
        Assert.NotEmpty(results);
        int lenght = results is null?0:results.Count();
        Assert.Equal(6, lenght);
    }


}