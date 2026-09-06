using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using ProductManager.API.Controllers;
using ProductManager.BAL.DTO;
using ProductManager.BAL.Exceptions;
using ProductManager.BAL.Services.Interfaces;
using ProductManager.API.Contracts;

namespace ProductManager.API.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _serviceMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _serviceMock = new Mock<IProductService>();
        _controller = new ProductsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenProductExists()
    {
        var product = new ProductDTO { Name = "PRD", Quantity = 8 };

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
            .ReturnsAsync((ProductDTO?)null);

        var result = await _controller.Get(999, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetAll_ReturnsOkAndProducts_WhenProductsExist()
    {
        IEnumerable<ProductDTO> products = [
            new ProductDTO{ Id=1, Name="PRD1", Quantity=2},
            new ProductDTO{ Id=2, Name="PRD2", Quantity=3},
            new ProductDTO{ Id=3, Name="PRD3", Quantity=3},
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
        IEnumerable<ProductDTO> empty = [];
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
        IEnumerable<ProductDTO> products = [
            new ProductDTO{ Id=1, Name="PRD1", Quantity=2},
            new ProductDTO{ Id=2, Name="PRD2", Quantity=3},
            new ProductDTO{ Id=3, Name="PRD3", Quantity=3},
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
        IEnumerable<ProductDTO> empty = [];
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
        IEnumerable<ProductDTO> products = [
            new ProductDTO{ Id=1, Name="PRD1", Quantity=2},
            new ProductDTO{ Id=2, Name="PRD2", Quantity=3},
            new ProductDTO{ Id=3, Name="PRD3", Quantity=3},
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
        IEnumerable<ProductDTO> empty = [];
        _serviceMock
            .Setup(s => s.SearchByAsync(null, 1, 4, CancellationToken.None))
            .ReturnsAsync(empty);

        var result = await _controller.Search(new StockLevelQuery { Min = 1, Max = 4 }, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(empty, okResult.Value);
    }
}