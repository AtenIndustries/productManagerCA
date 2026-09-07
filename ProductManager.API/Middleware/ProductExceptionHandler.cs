using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductManager.BAL.Exceptions;

namespace ProductManager.API.Middleware;

public class ProductExceptionHandler(ILogger<ProductExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<ProductExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        // Deals with client cancelation
        if (exception is OperationCanceledException)
        {
            _logger.LogInformation("Request canceled by client");
            return true;
        }

        var (statusCode, title) = exception switch
        {
            ProductNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            ProductConcurrencyException => (StatusCodes.Status409Conflict, "Concurrency conflict"),
            DuplicateProductException => (StatusCodes.Status409Conflict, "Duplicate product"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Unhandled error");
        else
            _logger.LogWarning(exception, "Handled error");

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
        }, ct);

        return true;
    }
}