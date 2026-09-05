using System.ComponentModel.DataAnnotations;

namespace ProductManager.API.Contracts;


public class StockUpdateQuery
{
    [Range(0, int.MaxValue, ErrorMessage = "number cannot be negative")]
    public int Number {get;set;}
    [Range(0, int.MaxValue, ErrorMessage = "quantity cannot be negative")]
    public int Quantity {get;set;}
}