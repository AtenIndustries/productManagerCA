using System.ComponentModel.DataAnnotations;
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
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDTO>> Get(int id, CancellationToken ct)
    {
        ProductDTO? product = await _productService.GetAsync(id, ct);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> Get(CancellationToken ct)
    {
        IEnumerable<ProductDTO>? products = await _productService.GetAllAsync( ct);
        return Ok(products);
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


    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDTO>> Update(int id, ProductDTO product, CancellationToken ct)
    {
        //Important note: ProductDTO.id is a init set field, so verifying against the query id
        //will lead to false BadRequestReturns 
        ProductDTO updPrd = await _productService.UpdateAsync(id, product, ct);
        return Ok(updPrd);
    }


    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _productService.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id}/increment-stock/{quantity}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDTO>> IncrementStock([FromRoute]StockUpdateQuery stockUpdateQuery, CancellationToken ct)
    {
        return await AddQuantity(stockUpdateQuery.Id, stockUpdateQuery.Quantity, ct);
    }


    //Won't work directly from browser unless cors is configured.
    [HttpPost("{id}/decrement-stock/{quantity}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDTO>> DecrementStock([FromRoute]StockUpdateQuery stockUpdateQuery, CancellationToken ct)
    {
        return await AddQuantity(stockUpdateQuery.Id, stockUpdateQuery.Quantity*-1, ct);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductDTO>> Search([FromQuery] string? name, CancellationToken ct)
    {
        IEnumerable<ProductDTO>? products = await _productService.SearchByAsync(name, null, null, ct);
        return Ok(products);
    }

    [HttpGet("stock-level")]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)] 
    public async Task<ActionResult<ProductDTO>> Search([FromQuery] StockLevelQuery query, CancellationToken ct)
    {
        IEnumerable<ProductDTO>? products = await _productService.SearchByAsync(null, query.Min, query.Max, ct);
        return Ok(products);
    }


    private async Task<ActionResult<ProductDTO>> AddQuantity(int id, int quantity, CancellationToken ct)
    {
        ProductDTO? product = await _productService.GetAsync(id, ct);
        if (product is null) {
            return NotFound();
        } 
        product.Quantity+=quantity;
        product.Quantity = Math.Max(product.Quantity, 0); 
        ProductDTO prd = await _productService.UpdateAsync(id, product, ct);
        return Ok(prd);
    }

}