using System.ComponentModel.DataAnnotations;

namespace ProductManager.API.Contracts;


public class StockLevelQuery : IValidatableObject
{
    [Range(0, int.MaxValue, ErrorMessage = "min cannot be negative")]
    public int? Min {get;set;}
    [Range(0, int.MaxValue, ErrorMessage = "max cannot be negative")]
    public int? Max {get;set;}

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Min.HasValue && Max.HasValue && Min > Max)
        {
            yield return new ValidationResult(
                "min cannot be greater than max.",
                new[] { nameof(Min), nameof(Max) });
        }
    }
}