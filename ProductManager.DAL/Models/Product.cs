using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProductManager.DAL.Models;

[Index(nameof(Name), IsUnique = true)]
public class Product
{
    [Key]
    public int Id { get; set; }
    [Column(TypeName = "varchar(200)")]
    public required string Name { get; set; }
    [Column(TypeName = "nvarchar(500)")]
    public string? Description { get; set; }
    public int Quantity { get; set; } 
    public DateTime Created { get; set; } 
    public DateTime Updated { get; set; }

    [Timestamp]
    public byte[] ConcurrencyToken { get; set; }=[];
}

