using System.ComponentModel.DataAnnotations; 
using Newtonsoft.Json; 

namespace ProductManager.BAL.DTO;

public class UpdateProductDTO
{
    [Required(ErrorMessage = "Name is mandatory")]
    [MinLength(1, ErrorMessage = "Name should not be empty")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;
    [MaxLength(200, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Quantity should be 0 or positive")]
    public int Quantity { get; set; } 
}