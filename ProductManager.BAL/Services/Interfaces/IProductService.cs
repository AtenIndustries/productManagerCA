using System;
using ProductManager.BAL.DTO;
using ProductManager.DAL.Models;


namespace ProductManager.BAL.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>?> GetAllAsync(CancellationToken ct = default);
    Task<ProductDTO?> GetAsync(int numberr, CancellationToken ct = default);
    Task<int> CreateAsync(ProductDTO product, CancellationToken ct = default);
    Task<int> UpdateAsync(ProductDTO product, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<ProductDTO>?> SearchByAsync(string? name, int? min, int? max, CancellationToken ct = default);
}