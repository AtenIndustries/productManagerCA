using System;
using ProductManager.BAL.DTO;
using ProductManager.DAL.Models;


namespace ProductManager.BAL.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDTO>?> GetAllAsync(CancellationToken ct = default);
    Task<ProductDTO?> GetAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(ProductDTO product, CancellationToken ct = default);
    Task<ProductDTO> UpdateAsync(int id, ProductDTO product, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<ProductDTO>?> SearchByAsync(string? name, int? min, int? max, CancellationToken ct = default);
    Task<ProductDTO> AdjustStockAsync(int id, int delta, CancellationToken ct = default);
}