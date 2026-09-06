using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using ProductManager.API.Controllers;
using ProductManager.BAL.DTO;
using ProductManager.BAL.Exceptions;
using ProductManager.BAL.Services.Interfaces;
using ProductManager.API.Contracts;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using ProductManager.API.Tests.Support;


namespace ProductManager.API.Tests.Controllers;

public class ProductInvalidFieldsTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_MissingName_ResultsInBadRequest()
    {
        var payload = new { Quantity = 5 };
        var response = await _client.PostAsJsonAsync("/api/products", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_EmptyName_ResultsInBadRequest()
    {
        var payload = new { Name = string.Empty, Number = 1, Quantity = 5 };
        var response = await _client.PutAsJsonAsync("/api/products/1", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_NegativeQuantity_ResultsInBadRequest()
    {
        var payload = new { Quantity = -5 };
        var response = await _client.PostAsJsonAsync("/api/products", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


}