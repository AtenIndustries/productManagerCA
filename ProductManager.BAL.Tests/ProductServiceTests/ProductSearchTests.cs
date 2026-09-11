using Microsoft.EntityFrameworkCore;
using ProductManager.DAL;
using ProductManager.BAL.Services;
using ProductManager.BAL.DTO;

namespace ProductManager.BAL.Tests.ProductServiceTests;

public class ProductSearchTests :ProductServiceTests
{
    [Theory]
    [InlineData(1, "PRD1", null, null)]
    [InlineData(2, null, 3, 6)]
    [InlineData(2, null, 4, 5)]
    [InlineData(3, null, null, 5)]
    [InlineData(2, "PRD", 4, 5)]
    [InlineData(4, "PRD", null, null)]
    [InlineData(3, "PRD", 4, 8)]
    [InlineData(0, "CHEM", 0, 4)]
    [InlineData(1, "CHEM2", 1, null)]
    [InlineData(2, "CHEM", 1, null)]
    [InlineData(6, null, null, null)]
    public async Task SearchByAsync_VariousNameAndStockFilters_ReturnsExpectedResultCount(int expectedNumberOfResults, string? name, int? min, int? max)
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ProductService service = CreateProductService(ctx);
        IEnumerable<ProductDTO>? results = await service.SearchByAsync(name, min, max, CancellationToken.None);
        int numResults = results is null ? 0 : results.Count();
        Assert.Equal(expectedNumberOfResults, numResults);
    }


    [Theory]
    [InlineData(true, 1)]
    [InlineData(true, 2)]
    [InlineData(false, 7)]
    [InlineData(false, 99)]
    public async Task GetAsync_ExistingOrNonExistingId_ReturnsExpectedResult(bool expectResult, int searchId)
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ProductService service = CreateProductService(ctx);
        ProductDTO? prd = await service.GetAsync(searchId, CancellationToken.None);
        bool hasResult = prd is not null;
        Assert.Equal(expectResult, hasResult);
    }

    [Fact]
    public async Task GetAllAsync_NoFilter_ReturnsAllProducts()
    {
        await using var ctx = await ContextGenerators.CreateSimpleContextWithData();
        ProductService service = CreateProductService(ctx);
        IEnumerable<ProductDTO>? results = await service.GetAllAsync(CancellationToken.None);
        Assert.NotNull(results);
        Assert.NotEmpty(results);
        int lenght = results is null ? 0 : results.Count();
        Assert.Equal(6, lenght);
    }


}