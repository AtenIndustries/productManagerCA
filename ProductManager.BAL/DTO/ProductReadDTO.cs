using System.ComponentModel.DataAnnotations;
using ProductManager.DAL.Models;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using ProductManager.DAL.Migrations;
using ProductManager.CommonLib.Interfaces;

namespace ProductManager.BAL.DTO;

public class ProductReadDTO : ProductWriteDTO, IAuditable
{
    public int Id { get; private set; }
    public byte[] ConcurrencyToken { get; private set; } = [];
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

}