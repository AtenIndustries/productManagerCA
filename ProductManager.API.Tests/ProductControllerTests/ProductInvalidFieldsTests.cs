using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;
using ProductManager.API.Tests.Support;


namespace ProductManager.API.Tests.Controllers;

public class ProductInvalidFieldsTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();


    [Fact]
    public async Task Create_MissingName_ResultsInBadRequest()
    {
        await Authenticate();
        var payload = new { Quantity = 5 };
        var response = await _client.PostAsJsonAsync("/api/products", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task Update_EmptyName_ResultsInBadRequest()
    {
        await Authenticate();
        var payload = new { Name = string.Empty, Number = 1, Quantity = 5 };
        var response = await _client.PutAsJsonAsync("/api/products/1", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_NegativeQuantity_ResultsInBadRequest()
    {
        await Authenticate();
        var payload = new { Quantity = -5 };
        var response = await _client.PostAsJsonAsync("/api/products", payload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Theory]
    [InlineData(-1, 10)]
    [InlineData(1, -5)]
    [InlineData(-6, -2)]
    public async Task IncrementStock_InvalidRouteValues_ResultsInBadRequest(int id, int delta)
    {
        await Authenticate();
        var response = await _client.PostAsJsonAsync($"api/products/{id}/increment-stock/{delta}", new { });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(1, -5)]
    [InlineData(-6, -2)]
    public async Task DecrementStock_InvalidRouteValues_ResultsInBadRequest(int id, int delta)
    {
        await Authenticate();
        var response = await _client.PostAsJsonAsync($"api/products/{id}/decrement-stock/{delta}", new { });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(1, -5)]
    [InlineData(6, 2)]
    public async Task StockLevel_InvalidRouteValues_ResultsInBadRequest(int min, int max)
    {
        var response = await _client.GetAsync($"api/products/stock-level?min={min}&max={max}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task Authenticate()
    {
        await _client.PostAsJsonAsync("/api/auth/register", new { Username = "UT_TEST", Password = "UT_PASS" });
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { Username = "UT_TEST", Password = "UT_PASS" });
        var successLoginResponse = loginResponse.EnsureSuccessStatusCode();
        var loginBody = await successLoginResponse.Content.ReadAsStringAsync();
        var token = JObject.Parse(loginBody)["token"]?.ToString();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

}