using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog((context, config) =>
{
    config
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()    
        .Enrich.WithEnvironmentName()
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] [{MachineName}] {Message:lj}{NewLine}{Exception}");
});



builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString =
    builder.Configuration.GetConnectionString("defaultConnectionString")
        ?? throw new InvalidOperationException("Connection string"
        + "'defaultConnectionString' not found.");

builder.Services.AddDbContext<ProductManager.DAL.ProductManagerDBContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("ProductManager.DAL")));

builder.Services.AddScoped<ProductManager.BAL.Services.Interfaces.IProductService, ProductManager.BAL.Services.ProductService>();
builder.Services.AddExceptionHandler<ProductManager.API.Middleware.ProductExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();
