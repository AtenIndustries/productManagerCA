using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ProductManager.API.Contracts;


public class StockUpdateQuery
{
    /// <summary>
    /// Product id
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Negative id value")]
    [FromRoute(Name = "id")]
    public int Id { get; set; }

    /// <summary>
    /// Delta
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Negative delta value")]
    [FromRoute(Name = "delta")]
    public int Delta { get; set; }
}