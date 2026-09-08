using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace ProductManager.DAL.Models;

[Index(nameof(Username), IsUnique = true)]
public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required] 
    [Column(TypeName = "varchar(100)")]
    public string Username { get; set; } = string.Empty;
    
    [Required] 
    [Column(TypeName = "varchar(100)")]
    public string PasswordHash { get; set; } = string.Empty;
}