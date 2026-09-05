using Microsoft.AspNetCore.Mvc;
using ProductManager.API.Contracts;
using ProductManager.BAL.DTO;
using ProductManager.BAL.Services.Interfaces;
using ProductManager.DAL;
using ProductManager.DAL.Models;

namespace ProductManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService productService) : Controller
{
    private readonly IProductService _productService = productService;

    [HttpGet("{number}")]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDTO>> Get(int number, CancellationToken ct)
    {
        ProductDTO? product = await _productService.GetAsync(number, ct);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDTO>> Create(ProductDTO product, CancellationToken ct)
    {
        int id = await _productService.CreateAsync(product, ct);
        ProductDTO? created = await _productService.GetAsync(id, ct);
        return CreatedAtAction(nameof(Create), new { id }, created);
    }


    [HttpPut("{number}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int number, ProductDTO product, CancellationToken ct)
    {
        if (number != product.Number)
        {
            return BadRequest($"Route number of {number} doesn't match request body number");
        }
        await _productService.UpdateAsync(product, ct);
        return NoContent();
    }


    [HttpDelete("{number}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int number, CancellationToken ct)
    {
        await _productService.DeleteAsync(number, ct);
        return NoContent();
    }

    //Won't work directly from browser unless cors is configured.
    [HttpPost("{number}/increment-stock/{quantity}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDTO>> IncrementStock([FromRoute]StockUpdateQuery stockUpdateQuery, CancellationToken ct)
    {
        return await AddQuantity(stockUpdateQuery.Number, stockUpdateQuery.Quantity, ct);
    }


    //Won't work directly from browser unless cors is configured.
    [HttpPost("{number}/decrement-stock/{quantity}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDTO>> DecrementStock([FromRoute]StockUpdateQuery stockUpdateQuery, CancellationToken ct)
    {
        return await AddQuantity(stockUpdateQuery.Number, stockUpdateQuery.Quantity*-1, ct);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Search([FromQuery] string? name, CancellationToken ct)
    {
        IEnumerable<ProductDTO>? products = await _productService.SearchByAsync(name, null, null, ct);
        return products == null ? NotFound() : Ok(products);
    }

    [HttpGet("stock-level")]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Search([FromQuery] StockLevelQuery query, CancellationToken ct)
    {
        IEnumerable<ProductDTO>? products = await _productService.SearchByAsync(null, query.Min, query.Max, ct);
        return products == null ? NotFound() : Ok(products);
    }


    private async Task<ActionResult<ProductDTO>>  AddQuantity(int number, int quantity, CancellationToken ct)
    {
        ProductDTO? product = await _productService.GetAsync(number, ct);
        if (product is null) {
            return NotFound();
        } 
        product.Quantity+=quantity;
        product.Quantity = Math.Max(product.Quantity, 0); 
        await _productService.UpdateAsync(product, ct);
        return Ok();
    }

}