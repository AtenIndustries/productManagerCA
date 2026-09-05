using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProductManager.DAL.Models;

[Index(nameof(Name), IsUnique = true)]
public class Product
{
    [Key]
    public int Id { get; set; } 
    [Column("varchar(200)")]
    public required string Name { get; set; }
    public int Quantity { get; set; }
    [Column("created")]
    public DateTime Created { get; set; }
    [Column("updated")]
    public DateTime Updated { get; set; }
}

