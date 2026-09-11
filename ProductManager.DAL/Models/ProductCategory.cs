using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ProductManager.CommonLib.Interfaces;

namespace ProductManager.DAL.Models;

[Index(nameof(CategoryName), IsUnique = true)]
public class ProductCategory : IAuditable
{
    [Key]
    public int Id { get; set; }
    [Column(TypeName = "varchar(200)")]
    public required string CategoryName { get; set; }
    [Timestamp]
    public byte[] ConcurrencyToken { get; set; } = [];
    //Auditable fields
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }


    public ICollection<Product>? Products { get; set; }
}

