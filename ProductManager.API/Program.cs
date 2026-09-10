using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ProductManager.CommonLib.Interceptors;

var builder = WebApplication.CreateBuilder(args);
const string TokenSchemeId = "Bearer";

// Console logs
builder.Host.UseSerilog((context, config) =>
{
    config
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] [{MachineName}] {Message:lj}{NewLine}{Exception}");
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString =
    builder.Configuration.GetConnectionString("defaultConnectionString") ;

if (string.IsNullOrEmpty(connectionString))
{
    // Verifies if execution comes from EF Core CLI
    if (EF.IsDesignTime)
    {
        connectionString = "Server=design-time-fake;Database=FakeDB;User Id=fake;Password=fake;Encrypt=True;";
    }
    else
    {
        throw new InvalidOperationException("Connection string 'defaultConnectionString' not found.");
    }
}

builder.Services.AddDbContext<ProductManager.DAL.ProductManagerDBContext>((sp, options) =>
    options.UseSqlServer(connectionString, b =>
    {
        b.MigrationsAssembly("ProductManager.DAL");
        b.EnableRetryOnFailure(maxRetryCount:5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd:null);
    })
    //Adds interceptor to fill audit fields for IAuditable entities
    .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));

// Configuration of JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.AddAuthorization();
builder.Services.AddControllers();

//Configuration of Automapper
builder.Services.AddAutoMapper(am=>{am.LicenseKey=builder.Configuration["AutomapperLicenceKey"];}, typeof(Program).Assembly);
builder.Services.AddScoped<ProductManager.BAL.Services.Interfaces.IProductService, ProductManager.BAL.Services.ProductService>();
builder.Services.AddScoped<ProductManager.BAL.Services.Interfaces.IUserService, ProductManager.BAL.Services.UserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuditInterceptor>();
builder.Services.AddExceptionHandler<ProductManager.API.Middleware.ProductExceptionHandler>();
builder.Services.AddProblemDetails(); //For detailed problem response in middle ware

//Service to build an API UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token below."
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(TokenSchemeId, document)] = []
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseSwagger();

app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductManager.API v1");
    s.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();

public partial class Program { }