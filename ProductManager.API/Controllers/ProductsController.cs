using Microsoft.AspNetCore.Mvc;
using ProductManager.DAL;
using ProductManager.DAL.Models;

namespace ProductManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : Controller
{
    private readonly ProductManagerDBContext _dbContext;

    public ProductsController(ProductManagerDBContext dBContext)
    {
        _dbContext=dBContext;
    }
 
}