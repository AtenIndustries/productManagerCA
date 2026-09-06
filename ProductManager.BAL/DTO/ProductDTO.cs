using System.ComponentModel.DataAnnotations;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.DTO;

public class ProductDTO
{
    public int Id { get; private set; }
    [Required(ErrorMessage = "Name is mandatory")]
    [MinLength(1, ErrorMessage = "Name should not be empty")]
    [MaxLength(200, ErrorMessage ="Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;
    [Range(0, int.MaxValue, ErrorMessage = "Quantity should be 0 or positive")]
    public int Quantity { get; set; }
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