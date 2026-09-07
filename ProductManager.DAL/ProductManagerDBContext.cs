using Microsoft.EntityFrameworkCore;
using ProductManager.DAL.Models;
namespace ProductManager.DAL;

public class ProductManagerDBContext(DbContextOptions<ProductManagerDBContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Generates a sequence of ids for Product Id creation
        modelBuilder.HasSequence<int>("ProductIds", schema: "dbo")
            .StartsAt(100000)
            .IncrementsBy(1)
            .HasMax(999999)
            .IsCyclic(false);

        modelBuilder.Entity<Product>()
            .ToTable(tb => tb.IsTemporal()) //creates history
            .Property(p => p.Id)
            .HasDefaultValueSql("NEXT VALUE FOR dbo.ProductIds", "DF_Products_Id"); //Adds a default constaint name to avoid migration problems on drop
    }
}