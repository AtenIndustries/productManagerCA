using System.Data;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProductManager.BAL.DTO;
using ProductManager.BAL.Exceptions;
using ProductManager.BAL.Services.Interfaces;
using ProductManager.DAL;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.Services;

public class ProductService(ProductManagerDBContext ctx, IMapper mapper) : IProductService
{
    private readonly ProductManagerDBContext _ctx = ctx;
    private readonly IMapper _mapper = mapper;

    public async Task<ProductReadDTO?> GetAsync(int id, CancellationToken ct = default)
    {
        Product? entity = await _ctx.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
        return entity is null ? null : _mapper.Map<ProductReadDTO>(entity);
    }

    public async Task<IEnumerable<ProductReadDTO>?> GetAllAsync(CancellationToken ct = default)
    {
        IEnumerable<ProductReadDTO>? entities = await _ctx.Products.AsNoTracking().ProjectTo<ProductReadDTO>(_mapper.ConfigurationProvider).ToListAsync(ct);
        return entities;
    }

    public async Task<int> CreateAsync(ProductWriteDTO writeData, CancellationToken ct = default)
    {
        Product entity = _mapper.Map<Product>(writeData);
        entity.Quantity = Math.Max(entity.Quantity, 0);
        _ctx.Add(entity);
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ProductConcurrencyException(writeData.Name, ex);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new DuplicateProductException(writeData, ex);
        }
        catch (DbUpdateException ex)
        {
            throw new ProductPersistenceException(writeData.Name, ex);
        }

        return entity.Id;
    }

    public async Task<ProductReadDTO> UpdateAsync(int id, ProductWriteDTO updateData, CancellationToken ct = default)
    {
        Product? entity = await _ctx.Products.FirstOrDefaultAsync(p => p.Id == id, ct)
        ?? throw new ProductNotFoundException(id);

        _ctx.Entry(entity).CurrentValues.SetValues(updateData);
        entity.Quantity = Math.Max(entity.Quantity, 0);//Prevent negative values 

        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ProductConcurrencyException(id, ex);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new DuplicateProductException(updateData, ex);
        }
        catch (DbUpdateException ex)
        {
            throw new ProductPersistenceException(id, ex);
        }
        return _mapper.Map<ProductReadDTO>(entity);
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

    public async Task<IEnumerable<ProductReadDTO>?> SearchByAsync(string? name, int? min, int? max, CancellationToken ct = default)
    {
        List<ProductReadDTO>? products = await _ctx.Products.AsNoTracking()
                .Where(p => (name == null || p.Name.ToLower() == name.ToLower() || p.Name.ToLower().Contains(name.ToLower()))
                && (min == null || p.Quantity >= min) && (max == null || p.Quantity <= max))
                .ProjectTo<ProductReadDTO>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);
        return products;
    }

    public async Task<ProductReadDTO> AdjustStockAsync(int id, int delta, CancellationToken ct = default)
    {
        Product entity = await _ctx.Products.FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new ProductNotFoundException(id);

        entity.Quantity = Math.Max(entity.Quantity + delta, 0);

        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ProductConcurrencyException(id, ex);
        }

        return _mapper.Map<ProductReadDTO>(entity);
    }
}