using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductManager.DAL;

/// <summary>
/// Sets a design time connection string for Github actions 
/// </summary>
public class ProductManagerDBContextFactory : IDesignTimeDbContextFactory<ProductManagerDBContext>
{
    public ProductManagerDBContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProductManagerDBContext>();
        
        var connectionString = Environment.GetEnvironmentVariable("EF_MIGRATIONS_CONNECTION")
            ?? "Server=design-time-fake;Database=FakeDB;User Id=fake;Password=fake;Encrypt=True;";
        
        optionsBuilder.UseSqlServer(connectionString);
        
        return new ProductManagerDBContext(optionsBuilder.Options);
    }
}