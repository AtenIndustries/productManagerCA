using System.ComponentModel.DataAnnotations;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.DTO;

public class ProductDTO(Product product)
{
    public int Id { get; set; } = product.Id;
    public int Number { get; set; } = product.Number;
    public string Name { get; set; } = product.Name;
    public int Quantity { get; set; } = product.Quantity;
    public DateTime Created { get; set; } = product.Created;
    public DateTime Updated { get; set; } = product.Updated;

    public Product ToEntity()
    {
        return new Product
        {
            Id = Id,
            Number = Number,
            Name = Name,
            Quantity = Quantity,
            Created = Created,
            Updated = Updated
        };
    }


}