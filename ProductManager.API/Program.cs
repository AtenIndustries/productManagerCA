using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString =
    builder.Configuration.GetConnectionString("defaultConnectionString")
        ?? throw new InvalidOperationException("Connection string"
        + "'defaultConnectionString' not found.");

builder.Services.AddDbContext<ProductManager.DAL.ProductManagerDBContext>(options =>
    options.UseSqlServer(connectionString, b=>b.MigrationsAssembly("ProductManager.DAL")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
