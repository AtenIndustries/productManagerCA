using System.Reflection;
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
builder.Services.AddProblemDetails(); //For detailed problem response in middle ware

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);   
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseSwagger();

app.UseSwaggerUI(s=>{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductManager.API v1");
    s.RoutePrefix = string.Empty;
});

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();

public partial class Program { }