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

    public async Task<ProductDTO?> GetAsync(int number, CancellationToken ct = default)
    {
        Product? entity = await _ctx.Products.FirstOrDefaultAsync(p => p.Number == number, ct);
        return entity is null ? null: ProductDTO.FromEntity(entity);
    }


    public async Task<int> CreateAsync(ProductDTO productDTO, CancellationToken ct = default)
    {
        Product entity = productDTO.ToEntity();
        entity.Created = DateTime.UtcNow;
        _ctx.Add(entity);
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ProductConcurrencyException(productDTO.Number, ex);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new DuplicateProductException(productDTO, ex);
        }
        catch (DbUpdateException ex)
        {
            throw new ProductPersistenceException(productDTO.Number, ex);
        }

        return entity.Id;
    }

    public async Task<int> UpdateAsync(ProductDTO productDTO, CancellationToken ct = default)
    {
        Product? entity = await _ctx.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Number == productDTO.Number, ct)
        ?? throw new ProductNotFoundException(productDTO.Number);

        Product updatedEntity = productDTO.ToEntity();
        updatedEntity.Id = entity.Id;
        updatedEntity.Created = entity.Created;
        updatedEntity.Updated = DateTime.UtcNow;
        _ctx.Update(updatedEntity);
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ProductConcurrencyException(productDTO.Number, ex);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new DuplicateProductException(productDTO, ex);
        }
        catch (DbUpdateException ex)
        {
            throw new ProductPersistenceException(productDTO.Number, ex);
        }
        return entity.Id;
    }

    public async Task<bool> DeleteAsync(int number, CancellationToken ct = default)
    {
        Product? entity = await _ctx.Products.FirstOrDefaultAsync(p => p.Number == number, ct)
        ?? throw new ProductNotFoundException(number);

        _ctx.Remove(entity);
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (ProductConcurrencyException ex)
        {
            throw new ProductConcurrencyException(number, ex);
        }
        catch (DbUpdateException ex)
        {
            throw new ProductPersistenceException(number, ex);
        }

        return true;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx
            && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
    }
}