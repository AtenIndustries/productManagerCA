using System.ComponentModel.DataAnnotations;
using ProductManager.DAL.Models;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using ProductManager.DAL.Migrations;

namespace ProductManager.BAL.DTO;

public class ProductDTO : ProductDataDTO
{
    public int Id { get; private set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public byte[] ConcurrencyToken { get; private set; } = [];

    public static ProductDTO FromEntity(Product product)
    {
        return new ProductDTO
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Quantity = product.Quantity,
            Created = product.Created,
            Updated = product.Updated,
            ConcurrencyToken = product.ConcurrencyToken
        };
    }

    public Product ToEntity()
    {
        return new Product
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Quantity = Quantity,
            Created = Created,
            Updated = Updated
        };
    }


}