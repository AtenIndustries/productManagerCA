using System.ComponentModel.DataAnnotations;

using ProductManager.BAL.DTO;

using Xunit;



namespace ProductManager.BAL.Tests.DTOTests;

public class ProductDTOTests
{
    private static List<ValidationResult> Validate(ProductDTO dto)
    {
        var context = new ValidationContext(dto);
        List<ValidationResult> results = [];
        Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void ProductDTO_EmptyName_FailsValidation()
    {
        var dto = new ProductDTO { Name = "", Quantity = 2 };
        var results = Validate(dto);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(ProductDTO.Name)));
    }

    [Fact]
    public void ProductDTO_NegativeQuantity_FailsValidation()
    {
        var dto = new ProductDTO { Name = "PRD1", Quantity = -1 };
        var results = Validate(dto);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(ProductDTO.Quantity)));
    }

    [Fact]
    public void ProductDTO_ValidData_PassesValidation()
    {
        var dto = new ProductDTO { Name = "PRD1", Quantity = 5 };
        var results = Validate(dto);
        Assert.Empty(results);
    }
}