using System;
using ProductManager.BAL.DTO;
using ProductManager.DAL.Models;


namespace ProductManager.BAL.Services.Interfaces;

public interface IProductService
{
    Task<ProductDTO> GetAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(ProductDTO author, CancellationToken ct = default);
    Task<int> UpdateAsync(ProductDTO author, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}