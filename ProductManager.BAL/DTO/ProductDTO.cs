using System.ComponentModel.DataAnnotations;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.DTO;

public class ProductDTO
{
    public int Id { get; private set; }       
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }   //this property should be filtered out from requests
    public DateTime Created { get; set; } 
    public DateTime Updated { get; set; }  

    public static ProductDTO FromEntity(Product product)
    {
        return new ProductDTO
        {
            Id = product.Id, 
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
            Name = Name,
            Quantity = Quantity,
            Created = Created,
            Updated = Updated
        };
    }


}