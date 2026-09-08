using Moq;
using ProductManager.BAL.Exceptions;
using Microsoft.Extensions.Logging;
using ProductManager.API.Middleware;
using Microsoft.AspNetCore.Http;

namespace ProductManager.API.Tests.MiddlewareTests;

public class ProductExceptionHandlerTests
{
    private readonly Mock<ILogger<ProductExceptionHandler>> _loggerMock;
    private readonly ProductExceptionHandler _productExceptionHandler;

    public ProductExceptionHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ProductExceptionHandler>>();
        _productExceptionHandler = new(_loggerMock.Object);
    }

    [Fact]
    public async Task TryHandleAsync_OnProductNotFoundException_Throws404NotFound()
    {
        ProductNotFoundException exception = new();
        HttpContext context = new DefaultHttpContext();
        await _productExceptionHandler.TryHandleAsync(context, exception, CancellationToken.None);
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_OnProductConcurrencyException_Throws409Conflit()
    {
        ProductConcurrencyException exception = new();
        HttpContext context = new DefaultHttpContext();
        await _productExceptionHandler.TryHandleAsync(context, exception, CancellationToken.None);
        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_OnDuplicateProductException_Throws409Conflit()
    {
        DuplicateProductException exception = new();
        HttpContext context = new DefaultHttpContext();
        await _productExceptionHandler.TryHandleAsync(context, exception, CancellationToken.None);
        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_OnUnhandledExceptions_Throws500InternalServerError()
    {
        Exception exception = new();
        HttpContext context = new DefaultHttpContext();
        await _productExceptionHandler.TryHandleAsync(context, exception, CancellationToken.None);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }

}