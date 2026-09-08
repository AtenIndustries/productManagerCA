using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.DAL;
using ProductManager.BAL.DTO;
using ProductManager.BAL.Services.Interfaces;
using ProductManager.BAL.Services;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace ProductManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ProductManagerDBContext context, IUserService userService, IConfiguration config) : ControllerBase
{
    private readonly ProductManagerDBContext _context = context;
    private readonly IUserService _userService = userService;
    private readonly IConfiguration _config = config;


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDTO request, CancellationToken ct)
    {
        if (await userService.HasUser(request.Username, ct))
        {
            return BadRequest("User already exists");
        }
        await _userService.RegisterUser(request, ct);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDTO request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user == null)
        {
            return BadRequest("Incorrect Username or Password.");
        }
 
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return BadRequest("Incorrect Username or Password.");
        }
 
        var token = GenerateJwtToken(user.Username);
        return Ok(new { Token = token });
    }


    private string GenerateJwtToken(string username)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(20),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}