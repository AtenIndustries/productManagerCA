using Microsoft.EntityFrameworkCore;
using ProductManager.BAL.DTO;
using ProductManager.BAL.Services.Interfaces;
using ProductManager.DAL;
using ProductManager.DAL.Models;

namespace ProductManager.BAL.Services;

public class UserService(ProductManagerDBContext ctx) : IUserService
{
    private readonly ProductManagerDBContext _ctx = ctx;

    public async Task<bool> HasUser(string username, CancellationToken ct = default)
    {
        return await _ctx.Users.AnyAsync(u => u.Username == username, ct);
    }

    public async Task RegisterUser(string username, string password, CancellationToken ct = default)
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var newUser = new User
        {
            Username = username,
            PasswordHash = passwordHash
        };
        _ctx.Users.Add(newUser);
        await _ctx.SaveChangesAsync(ct);
    }
}