using System.Data;
using Microsoft.EntityFrameworkCore;
using ProductManager.BAL.DTO;
using ProductManager.BAL.Exceptions;
using ProductManager.BAL.Services.Interfaces;
using ProductManager.DAL;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.Services;

public class ProductService(ProductManagerDBContext ctx) : IProductService
{
    private readonly ProductManagerDBContext _ctx = ctx;

    public async Task<ProductDTO?> GetAsync(int id, CancellationToken ct = default)
    {
        Product? entity = await _ctx.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
        return entity is null ? null : ProductDTO.FromEntity(entity);
    }

    public async Task<IEnumerable<ProductDTO>?> GetAllAsync(CancellationToken ct = default)
    {
        IEnumerable<ProductDTO>? entities = await _ctx.Products.AsNoTracking().Select(p => ProductDTO.FromEntity(p)).ToListAsync(ct);
        return entities;
    }

    public async Task<int> CreateAsync(ProductDTO productDTO, CancellationToken ct = default)
    {
        Product entity = productDTO.ToEntity();
        entity.Quantity = Math.Max(entity.Quantity, 0);
        entity.Created = DateTime.UtcNow;
        _ctx.Add(entity);
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ProductConcurrencyException(productDTO.Id, ex);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new DuplicateProductException(productDTO, ex);
        }
        catch (DbUpdateException ex)
        {
            throw new ProductPersistenceException(productDTO.Id, ex);
        }

        return entity.Id;
    }

    public async Task<ProductDTO> UpdateAsync(int id, ProductDTO productDTO, CancellationToken ct = default)
    {
        Product? entity = await _ctx.Products.FirstOrDefaultAsync(p => p.Id == id, ct)
        ?? throw new ProductNotFoundException(productDTO.Id);

        //Note: The in memory database does not care about Unique constrainsts.
        //      Just a piece of code to pass some unit tests.
        bool duplicateExists = await _ctx.Products
            .AnyAsync(p => p.Name == productDTO.Name && p.Id != id, ct);
        if (duplicateExists)
        {
            throw new DuplicateProductException(productDTO);
        }

        entity.Name = productDTO.Name;
        entity.Quantity = Math.Max(productDTO.Quantity, 0);//Prevent negative values
        entity.Updated = DateTime.Now;

        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ProductConcurrencyException(productDTO.Id, ex);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new DuplicateProductException(productDTO, ex);
        }
        catch (DbUpdateException ex)
        {
            throw new ProductPersistenceException(productDTO.Id, ex);
        }
        return ProductDTO.FromEntity(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        try
        {
            Product? entity = await _ctx.Products.FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new ProductNotFoundException(id);

            _ctx.Remove(entity);

            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ProductConcurrencyException(id, ex);
        }
        catch (DbUpdateException ex)
        {
            throw new ProductPersistenceException(id, ex);
        }

        return true;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
            && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
    }

    public async Task<IEnumerable<ProductDTO>?> SearchByAsync(string? name, int? min, int? max, CancellationToken ct = default)
    {
        List<ProductDTO>? products = await _ctx.Products.AsNoTracking()
                .Where(p => (name == null || p.Name.ToLower() == name.ToLower() || p.Name.ToLower().Contains(name.ToLower()))
                && (min == null || p.Quantity >= min) && (max == null || p.Quantity <= max))
                .Select(p => ProductDTO.FromEntity(p))
                .ToListAsync(ct);
        return products;
    }

    public async Task<ProductDTO> AdjustStockAsync(int id, int delta, CancellationToken ct = default)
    {
        Product entity = await _ctx.Products.FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new ProductNotFoundException(id);

        entity.Quantity = Math.Max(entity.Quantity + delta, 0);
        entity.Updated = DateTime.UtcNow;

        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ProductConcurrencyException(id, ex);
        }

        return ProductDTO.FromEntity(entity);
    }
}