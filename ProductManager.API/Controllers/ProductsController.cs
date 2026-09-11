using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManager.API.Contracts;
using ProductManager.BAL.DTO;
using ProductManager.BAL.Services.Interfaces;

namespace ProductManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService productService, IMapper mapper) : Controller
{
    private readonly IProductService _productService = productService;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Gets product by id
    /// </summary>
    /// <param name="id">Product id</param>
    /// <param name="ct"></param> 
    /// <returns>Product with a given id</returns> 
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductReadDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductReadDTO>> Get(int id, CancellationToken ct)
    {
        ProductReadDTO? product = await _productService.GetAsync(id, ct);
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>
    /// Gets all products
    /// </summary> 
    /// <returns>List of found products</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ProductReadDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProductReadDTO>>> Get(CancellationToken ct)
    {
        IEnumerable<ProductReadDTO>? products = await _productService.GetAllAsync(ct);
        return Ok(products);
    }

    /// <summary>
    /// Creates product
    /// </summary>
    /// <param name="product"></param>
    /// <param name="ct"></param>
    /// <returns>Created product</returns>  
    /// <response code="200">Product created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized — valid JWT token required.</response>
    /// <response code="409">Product already exists.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ProductReadDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductReadDTO>> Create(ProductDataBody product, CancellationToken ct)
    {
        ProductWriteDTO createDataDto = _mapper.Map<ProductWriteDTO>(product);
        int id = await _productService.CreateAsync(createDataDto, ct);
        ProductReadDTO? created = await _productService.GetAsync(id, ct);
        return CreatedAtAction(nameof(Create), new { id }, created);
    }

    /// <summary>
    /// Updates product
    /// </summary>
    /// <param name="id">Product id</param>
    /// <param name="product">Product data</param>
    /// <param name="ct"></param>
    /// <returns>Product updated</returns> 
    /// <response code="200">Product updated successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized — valid JWT token required.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="409">Product already exists.</response>    
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProductReadDTO>> Update(int id, ProductDataBody product, CancellationToken ct)
    {
        ProductReadDTO updPrd = await _productService.UpdateAsync(id, _mapper.Map<ProductWriteDTO>(product), ct);
        return Ok(updPrd);
    }

    /// <summary>
    /// Deletes product
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns></returns> 
    /// <response code="204">Product deleted with success</response>
    /// <response code="401">Unauthorized — valid JWT token required.</response>
    /// <response code="404">Product not found.</response> 
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized — valid JWT token required.</response>
    /// <response code="404">Product not found.</response> 
    [HttpPost("{id}/increment-stock/{delta}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductReadDTO>> IncrementStock([FromRoute] StockUpdateQuery stockUpdateQuery, CancellationToken ct)
    {
        return Ok(await _productService.AdjustStockAsync(stockUpdateQuery.Id, stockUpdateQuery.Delta, ct));
    }


    /// <summary>
    /// Increments stock
    /// </summary>
    /// <param name="stockUpdateQuery"></param>
    /// <param name="ct"></param>
    /// <returns></returns> 
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized — valid JWT token required.</response>
    /// <response code="404">Product not found.</response> 
    [HttpPost("{id}/decrement-stock/{delta}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductReadDTO>> DecrementStock([FromRoute] StockUpdateQuery stockUpdateQuery, CancellationToken ct)
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
    [ProducesResponseType(typeof(ProductReadDTO), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductReadDTO>> Search([FromQuery] string? name, CancellationToken ct)
    {
        IEnumerable<ProductReadDTO>? products = await _productService.SearchByAsync(name, null, null, ct);
        return Ok(products);
    }

    /// <summary>
    /// Search products by stock level
    /// </summary>
    /// <param name="query"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <response code="400">Invalid request data.</response>
    [HttpGet("stock-level")]
    [ProducesResponseType(typeof(ProductReadDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductReadDTO>> Search([FromQuery] StockLevelQuery query, CancellationToken ct)
    {
        IEnumerable<ProductReadDTO>? products = await _productService.SearchByAsync(null, query.Min, query.Max, ct);
        return Ok(products);
    }
}