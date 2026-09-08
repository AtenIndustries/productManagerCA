using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ProductManager.CommonLib.Interfaces;

namespace ProductManager.DAL.Models;

[Index(nameof(Name), IsUnique = true)]
public class Product : IAuditable
{
    [Key]
    public int Id { get; set; }
    [Column(TypeName = "varchar(200)")]
    public required string Name { get; set; }
    [Column(TypeName = "nvarchar(500)")]
    public string? Description { get; set; }
    public int Quantity { get; set; }

    [Timestamp]
    public byte[] ConcurrencyToken { get; set; } = [];

    //Auditable fields
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

