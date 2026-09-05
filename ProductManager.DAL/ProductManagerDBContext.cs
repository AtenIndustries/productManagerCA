using Microsoft.EntityFrameworkCore;
using ProductManager.DAL.Models;
namespace ProductManager.DAL;

public class ProductManagerDBContext(DbContextOptions<ProductManagerDBContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
}