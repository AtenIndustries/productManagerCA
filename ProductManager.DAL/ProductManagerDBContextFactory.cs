using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

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
            ?? BuildConfigurationConnectionString();

        Console.WriteLine("Design time connection string:"+connectionString);
        optionsBuilder.UseSqlServer(connectionString);

        return new ProductManagerDBContext(optionsBuilder.Options);
    }

    private static string BuildConfigurationConnectionString()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        return config.GetConnectionString("defaultConnectionString")
            ?? throw new InvalidOperationException("No connection string found for design-time operations.");
    }
}