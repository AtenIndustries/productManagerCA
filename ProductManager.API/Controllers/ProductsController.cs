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

    /// <summary>
    /// Gets product by id
    /// </summary>
    /// <param name="id">Product id</param>
    /// <param name="ct"></param> 
    /// <returns>Product with a given id</returns> 
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDTO>> Get(int id, CancellationToken ct)
    {
        ProductDTO? product = await _productService.GetAsync(id, ct);
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>
    /// Gets all products
    /// </summary> 
    /// <returns>List of found products</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> Get(CancellationToken ct)
    {
        IEnumerable<ProductDTO>? products = await _productService.GetAllAsync(ct);
        return Ok(products);
    }

    /// <summary>
    /// Creates product
    /// </summary>
    /// <param name="product"></param>
    /// <param name="ct"></param>
    /// <returns>Created product</returns>  
    [HttpPost]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDTO>> Create(ProductDTO product, CancellationToken ct)
    {
        int id = await _productService.CreateAsync(product, ct);
        ProductDTO? created = await _productService.GetAsync(id, ct);
        return CreatedAtAction(nameof(Create), new { id }, created);
    }

    /// <summary>
    /// Updates product
    /// </summary>
    /// <param name="id">Product id</param>
    /// <param name="product">Product data</param>
    /// <param name="ct"></param>
    /// <returns>Product updated</returns> 
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDTO>> Update(int id, ProductDTO product, CancellationToken ct)
    {
        ProductDTO updPrd = await _productService.UpdateAsync(id, product, ct);
        return Ok(updPrd);
    }

    /// <summary>
    /// Deletes product
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _productService.DeleteAsync(id, ct);
        return NoContent();
    }

    /// <summary>
    /// Increments stock
    /// </summary>
    /// <param name="stockUpdateQuery"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("{id}/increment-stock/{quantity}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDTO>> IncrementStock([FromRoute] StockUpdateQuery stockUpdateQuery, CancellationToken ct)
    {
        return Ok(await _productService.AdjustStockAsync(stockUpdateQuery.Id, stockUpdateQuery.Delta, ct));
    }


    /// <summary>
    /// Decrements stock
    /// </summary>
    /// <param name="stockUpdateQuery"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("{id}/decrement-stock/{quantity}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDTO>> DecrementStock([FromRoute] StockUpdateQuery stockUpdateQuery, CancellationToken ct)
    {
        return Ok(await _productService.AdjustStockAsync(stockUpdateQuery.Id, -1 * stockUpdateQuery.Delta, ct));
    }


    /// <summary>
    /// Search products by name stock
    /// </summary>
    /// <param name="name">Minimum value</param> 
    /// <param name="ct"></param>
    /// <returns>Product updated</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductDTO>> Search([FromQuery] string? name, CancellationToken ct)
    {
        IEnumerable<ProductDTO>? products = await _productService.SearchByAsync(name, null, null, ct);
        return Ok(products);
    }

    /// <summary>
    /// Search products by stock level
    /// </summary>
    /// <param name="query"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("stock-level")]
    [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductDTO>> Search([FromQuery] StockLevelQuery query, CancellationToken ct)
    {
        IEnumerable<ProductDTO>? products = await _productService.SearchByAsync(null, query.Min, query.Max, ct);
        return Ok(products);
    }
}