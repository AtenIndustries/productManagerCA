using System;
using ProductManager.BAL.DTO;
using ProductManager.DAL.Models;


namespace ProductManager.BAL.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductReadDTO>?> GetAllAsync(CancellationToken ct = default);
    Task<ProductReadDTO?> GetAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(ProductWriteDTO createData, CancellationToken ct = default);
    Task<ProductReadDTO> UpdateAsync(int id, ProductWriteDTO updateData, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<ProductReadDTO>?> SearchByAsync(string? name, int? min, int? max, CancellationToken ct = default);
    Task<ProductReadDTO> AdjustStockAsync(int id, int delta, CancellationToken ct = default);
}