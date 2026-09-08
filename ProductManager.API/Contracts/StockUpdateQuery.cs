using System.ComponentModel.DataAnnotations;

namespace ProductManager.API.Contracts;


public class StockUpdateQuery
{
    /// <summary>
    /// Product id
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Negative id value")]
    public int Id { get; set; }

    /// <summary>
    /// Delta
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Negative delta value")]
    public int Delta { get; set; }
}