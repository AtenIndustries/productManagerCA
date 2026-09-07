using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProductManager.API.Tests.Support;
using ProductManager.BAL.DTO;
using Reqnroll;
using Xunit;

namespace ProductManager.API.Tests.Steps;

[Binding]
public class StockManagementSteps
{
    private readonly HttpClient _client;
    private HttpResponseMessage? _response;
    private int _productId;

    public StockManagementSteps(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Given(@"exists product with name ""(.*)"" and quantity ""(.*)""")]
    public async Task GivenAnExistingProductWithQuantity(string name, string quantity)
    {
        //Creates the product
        var payload = new { Name = name, Quantity = int.Parse(quantity) };
        var response = await _client.PostAsJsonAsync("/api/products", payload);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        _productId = json.GetProperty("id").GetInt32();
    }

    [When(@"decrements ""(.*)"" units of that product")]
    public async Task WhenDecrementsStock(string delta)
    {
        _response = await _client.PostAsync($"/api/products/{_productId}/decrement-stock/{delta}", null);
    }

    [When(@"increments ""(.*)"" units of that product")]
    public async Task WhenIncrementsStock(string delta)
    {
        _response = await _client.PostAsync($"/api/products/{_productId}/increment-stock/{delta}", null);
    }

    [Then(@"the answer is ""(.*)""")]
    public void ThenTheAnswerIs(string expectedStatus)
    {
        Assert.Equal(expectedStatus, _response!.StatusCode.ToString());
    }

    [Then(@"product stock becomes ""(.*)""")]
    public async Task ThenTheStockIs(string expectedStock)
    { 
        var product = await _response!.Content.ReadFromJsonAsync<ProductDTO>(); 
        Assert.Equal(int.Parse(expectedStock), product!.Quantity);   
    }
}