using Microsoft.EntityFrameworkCore;
using ProductManager.BAL.Services;
using ProductManager.BAL.Exceptions;
using ProductManager.BAL.DTO;
using ProductManager.DAL.Models;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using ProductManager.DAL;

namespace ProductManager.BAL.Tests.ProductServiceTests;


public class ProductServiceTests
{
    private readonly IMapper _mapper;
    public ProductServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoMappingProfile>();
            }, NullLoggerFactory.Instance);
        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();
    }

    public ProductService CreateProductService(ProductManagerDBContext ctx)
    {
        return new ProductService(ctx, _mapper);
    }
}