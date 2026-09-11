using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.DTO;

public class ProductWriteDTO
{ 
    public string Name { get; set; } = string.Empty; 
    public string? Description { get; set; } 
    public int Quantity { get; set; }
 
}