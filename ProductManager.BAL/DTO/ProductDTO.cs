using System.ComponentModel.DataAnnotations;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.DTO;

public class ProductDTO
{
    public int Id { get; set; }  
    public int Number { get; set; }  
    public string Name { get; set; }  
    public int Quantity { get; set; }   //this property should be filtered out from requests
    public DateTime Created { get; set; } 
    public DateTime Updated { get; set; }  

    public static ProductDTO FromEntity(Product product)
    {
        return new ProductDTO
        {
            Id = product.Id,
            Number = product.Number,
            Name = product.Name,
            Quantity = product.Quantity,
            Created = product.Created,
            Updated = product.Updated
        };
    }

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