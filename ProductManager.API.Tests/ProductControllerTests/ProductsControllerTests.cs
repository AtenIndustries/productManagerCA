using Moq;
using Microsoft.AspNetCore.Mvc;
using ProductManager.API.Controllers;
using ProductManager.BAL.DTO;
using ProductManager.BAL.Services.Interfaces;
using ProductManager.API.Contracts;
using AutoMapper;
using ProductManager.API.Tests.Support;
using Microsoft.Extensions.DependencyInjection;

namespace ProductManager.API.Tests.ProductControllerTests;

public class ProductsSearchTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly Mock<IProductService> _serviceMock;
    private readonly ProductsController _controller;

    public ProductsSearchTests(ApiWebApplicationFactory factory)
    {
        _serviceMock = new Mock<IProductService>();
        IMapper _mapper = factory.Services.GetRequiredService<IMapper>();
        _controller = new ProductsController(_serviceMock.Object, _mapper);

    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenProductExists()
    {
        var product = new ProductReadDTO { Name = "PRD", Quantity = 8 };

        _serviceMock
            .Setup(s => s.GetAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _controller.Get(1, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(product, okResult.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        _serviceMock
            .Setup(s => s.GetAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductReadDTO?)null);

        var result = await _controller.Get(999, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAll_ReturnsOkAndProducts_WhenProductsExist()
    {
        IEnumerable<ProductReadDTO> products = [
            new ProductReadDTO{   Name="PRD1", Quantity=2},
            new ProductReadDTO{   Name="PRD2", Quantity=3},
            new ProductReadDTO{  Name="PRD3", Quantity=3},
        ];
        _serviceMock
            .Setup(s => s.GetAllAsync(CancellationToken.None))
            .ReturnsAsync(products);

        var result = await _controller.Get(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(products, okResult.Value);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_EvenWhenNoProductsExist()
    {
        IEnumerable<ProductReadDTO> empty = [];
        _serviceMock
            .Setup(s => s.GetAllAsync(CancellationToken.None))
            .ReturnsAsync(empty);

        var result = await _controller.Get(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(empty, okResult.Value);
    }


    [Fact]
    public async Task Search_ReturnsOk_WhenSearchHasResults()
    {
        IEnumerable<ProductReadDTO> products = [
            new ProductReadDTO{   Name="PRD1", Quantity=2},
            new ProductReadDTO{   Name="PRD2", Quantity=3},
            new ProductReadDTO{   Name="PRD3", Quantity=3},
        ];
        _serviceMock
            .Setup(s => s.SearchByAsync("PRD", null, null, CancellationToken.None))
            .ReturnsAsync(products);

        var result = await _controller.Search("PRD", CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(products, okResult.Value);
    }

    [Fact]
    public async Task Search_ReturnsOk_EvenWhenSearchReturnsNoResults()
    {
        IEnumerable<ProductReadDTO> empty = [];
        _serviceMock
            .Setup(s => s.SearchByAsync("PRD", null, null, CancellationToken.None))
            .ReturnsAsync(empty);

        var result = await _controller.Search("PRD", CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(empty, okResult.Value);
    }

    [Fact]
    public async Task SearchByStockLevel_ReturnsOk_WhenSearchHasResults()
    {
        IEnumerable<ProductReadDTO> products = [
            new ProductReadDTO{   Name="PRD1", Quantity=2},
            new ProductReadDTO{  Name="PRD2", Quantity=3},
            new ProductReadDTO{  Name="PRD3", Quantity=3},
        ];
        _serviceMock
            .Setup(s => s.SearchByAsync(null, 1, 4, CancellationToken.None))
            .ReturnsAsync(products);

        var result = await _controller.Search(new StockLevelQuery { Min = 1, Max = 4 }, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(products, okResult.Value);
    }
    [Fact]
    public async Task SearchByStockLevel_ReturnsOk_EvenWhenSearchReturnsNoResults()
    {
        IEnumerable<ProductReadDTO> empty = [];
        _serviceMock
            .Setup(s => s.SearchByAsync(null, 1, 4, CancellationToken.None))
            .ReturnsAsync(empty);

        var result = await _controller.Search(new StockLevelQuery { Min = 1, Max = 4 }, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(empty, okResult.Value);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenProductIsCreated()
    {
        int id = 1;
        ProductReadDTO prd = new() { Name = "PRD", Quantity = 3 };
        ProductDataBody prdReqBody = new() { Name = "PRD", Quantity = 3 };

        _serviceMock
            .Setup(s => s.CreateAsync(
                //The It.Is is needed because automapper creates new instances of ProductWriteDTO 
                It.Is<ProductWriteDTO>(p => p.Name == "PRD" && p.Quantity == 3),
                CancellationToken.None))
            .ReturnsAsync(id);

        var result = await _controller.Create(prdReqBody, CancellationToken.None);
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        int resultId = -1;
        if (createdResult is not null && createdResult.RouteValues is not null && createdResult.RouteValues["id"] is int v)
        {
            resultId = v;
        }
        Assert.Equal(id, resultId);
    }

    [Fact]
    public async Task Update_ReturnsCreated_WhenProductIsCreated()
    {
        int id = 1;
        ProductReadDTO prd = new() { Name = "PRD", Quantity = 3 };
        ProductDataBody prdReqBody = new() { Name = "PRD", Quantity = 3 };

        _serviceMock
            .Setup(s => s.CreateAsync(
                It.Is<ProductWriteDTO>(p => p.Name == "PRD" && p.Quantity == 3),
                CancellationToken.None))
            .ReturnsAsync(id);

        var result = await _controller.Create(prdReqBody, CancellationToken.None);
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        int resultId = -1;
        if (createdResult is not null && createdResult.RouteValues is not null && createdResult.RouteValues["id"] is int v)
        {
            resultId = v;
        }
        Assert.Equal(id, resultId);
    }


    [Fact]
    public async Task Update_ReturnsOk_WhenProductIsUpdated()
    {
        int id = 1;
        ProductDataBody updDataReqBody = new() { Name = "PRD", Quantity = 3 };
        ProductReadDTO prd = new() { Name = "PRD", Quantity = 3 };

        _serviceMock
            .Setup(s => s.UpdateAsync(id,
                It.Is<ProductWriteDTO>(p => p.Name == "PRD" && p.Quantity == 3), CancellationToken.None))
            .ReturnsAsync(prd);

        var result = await _controller.Update(id, updDataReqBody, CancellationToken.None);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(prd, okResult.Value);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenProductIsDeletedWithSuccess()
    {
        int id = 1;
        _serviceMock
            .Setup(s => s.DeleteAsync(id, CancellationToken.None))
            .ReturnsAsync(true);

        var result = await _controller.Delete(id, CancellationToken.None);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task IncrementStock_ReturnsOk_WhenStockIncrements()
    {
        int id = 1;
        ProductReadDTO prd = new() { Name = "PRD", Quantity = 3 };
        ProductReadDTO incrementedPrd = new() { Name = "PRD", Quantity = 4 };
        _serviceMock
            .Setup(s => s.AdjustStockAsync(id, 1, CancellationToken.None))
            .ReturnsAsync(incrementedPrd);

        var result = await _controller.IncrementStock(new StockUpdateQuery { Id = id, Delta = 1 }, CancellationToken.None);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(incrementedPrd, okResult.Value);
    }

    [Fact]
    public async Task DecrementStock_ReturnsOk_WhenStockIncrements()
    {
        int id = 1;
        ProductReadDTO prd = new() { Name = "PRD", Quantity = 3 };
        ProductReadDTO decrementedPrd = new() { Name = "PRD", Quantity = 2 };
        _serviceMock
            .Setup(s => s.AdjustStockAsync(id, -1, CancellationToken.None))
            .ReturnsAsync(decrementedPrd);

        var result = await _controller.DecrementStock(new StockUpdateQuery { Id = id, Delta = 1 }, CancellationToken.None);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(decrementedPrd, okResult.Value);
    }
}