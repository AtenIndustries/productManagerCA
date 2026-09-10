using System;
using ProductManager.BAL.DTO;
using ProductManager.DAL.Models;


namespace ProductManager.BAL.Services.Interfaces;

public interface IUserService
{
    Task<bool> HasUser(string username, CancellationToken ct = default);
    Task RegisterUser(string username, string password, CancellationToken ct = default);

}