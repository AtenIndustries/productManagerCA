using Microsoft.AspNetCore.Mvc;
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
}