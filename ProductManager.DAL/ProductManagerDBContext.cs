using Microsoft.EntityFrameworkCore;
using ProductManager.DAL.Models;
namespace ProductManager.DAL;

public class ProductManagerDBContext : DbContext
{
    public ProductManagerDBContext(DbContextOptions<ProductManagerDBContext> options) : base(options) { }
    public DbSet<Product> Products { get; set; }
}