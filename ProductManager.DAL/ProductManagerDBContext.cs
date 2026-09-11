using Microsoft.EntityFrameworkCore;
using ProductManager.DAL.Models;
namespace ProductManager.DAL;

public class ProductManagerDBContext(DbContextOptions<ProductManagerDBContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Generates a sequence of ids for Product Id creation
        modelBuilder.HasSequence<int>("ProductIds", schema: "dbo")
            .StartsAt(100006) //Starts at 100006 to avoid collisions with the test data
            .IncrementsBy(1)
            .HasMax(999999)
            .IsCyclic(false);

        modelBuilder.Entity<Product>()
            .ToTable(tb => tb.IsTemporal()) //creates history
            .Property(p => p.Id)
            .HasDefaultValueSql("NEXT VALUE FOR dbo.ProductIds", "DF_Products_Id"); //Adds a default constaint name to avoid migration problems on drop

        modelBuilder.Entity<ProductCategory>().HasData(
            new ProductCategory() { Id = 1, CategoryName = "Other", ConcurrencyToken = [1], CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "App" },
            new ProductCategory() { Id = 2, CategoryName = "Toys", ConcurrencyToken = [1], CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "App" },
            new ProductCategory() { Id = 3, CategoryName = "Appliances", ConcurrencyToken = [1], CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "App" },
            new ProductCategory() { Id = 4, CategoryName = "Grocery", ConcurrencyToken = [1], CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "App" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product() { Id = 100000, ProductCategoryId = 3, Name = "Dishwasher", Description = "Cheapest dishwasher ever", Quantity = 12, ConcurrencyToken = [1, 1, 1, 1], CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "App" },
            new Product() { Id = 100001, ProductCategoryId = 2, Name = "Slingshot", Description = "Wooden slignshot", Quantity = 1, ConcurrencyToken = [2, 2, 2, 2], CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "App" },
            new Product() { Id = 100002, ProductCategoryId = 2, Name = "DreadPool Action Figure", Description = "Action figure", Quantity = 8, ConcurrencyToken = [3, 3, 3, 3], CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "App" },
            new Product() { Id = 100004, ProductCategoryId = 4, Name = "Bag of fruits", Description = "Mix of fruits", Quantity = 25, ConcurrencyToken = [4, 4, 4, 4], CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "App" },
            new Product() { Id = 100005, ProductCategoryId = 4, Name = "Bag of nuts", Description = "Mix of nuts", Quantity = 20, ConcurrencyToken = [5, 5, 5, 5], CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), CreatedBy = "App" }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = Environment.GetEnvironmentVariable("EF_MIGRATIONS_CONNECTION")
               ?? "Server=design-time-fake;Database=FakeDB;User Id=fake;Password=fake;Encrypt=True;";
            optionsBuilder.UseSqlServer(
                connectionString,
                options => options.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null));
        }
    }
}