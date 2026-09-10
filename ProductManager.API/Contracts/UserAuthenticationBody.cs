using System.ComponentModel.DataAnnotations;

namespace ProductManager.API.Contracts;

public class UserAuthenticationBody
{
    [Required(ErrorMessage = "Username is mandatory")]
    [MinLength(5, ErrorMessage = "Username min length is 5")]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is mandatory")]
    [MinLength(5, ErrorMessage = "Password min length is 6")]
    public string Password { get; set; } = string.Empty;
}