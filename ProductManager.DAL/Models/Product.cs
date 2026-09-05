using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManager.DAL.Models;


public class Product
{
    [Key]
    public int Id { get; set; }
    public int Number { get; set; }
    [Column("varchar(200)")]
    public string Name { get; set; }
    public int Quantity { get; set; }
    [Column("created")]
    public DateTime Created { get; set; }
    [Column("updated")]
    public DateTime Updated { get; set; }
}

