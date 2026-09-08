using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ProductManager.API.Contracts;


public class StockLevelQuery : IValidatableObject
{
    /// <summary>
    /// Min value. Invalid if negative or bigger than max
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Negative min value")]
    [FromQuery(Name = "min")]
    public int? Min { get; set; }

    /// <summary>
    /// Max value. Invalid if negative or bigger than max
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Negative max value")]
    [FromQuery(Name = "max")]
    public int? Max { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Min.HasValue && Max.HasValue && Min > Max)
        {
            yield return new ValidationResult(
                "Min greater than max",
                [nameof(Min), nameof(Max)]);
        }
    }
}