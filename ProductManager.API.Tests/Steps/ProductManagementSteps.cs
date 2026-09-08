using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using ProductManager.API.Tests.Support;
using ProductManager.BAL.DTO;
using Reqnroll; 

namespace ProductManager.API.Tests.BddTestsSamples.Steps;

[Binding]
public class ProductManagementSteps
{
    private readonly HttpClient _client;
    private HttpResponseMessage? _response;
    private int _productId;

    public ProductManagementSteps(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Given(@"exists a registered user ""(.*)"" with password ""(.*)""")]
    public async Task GivenRegisteredUser(string username, string password)
    {
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register",
            new { Username = username, Password = password });
        registerResponse.EnsureSuccessStatusCode();
    }

    [Given(@"user is logged in with username ""(.*)"" and password ""(.*)""")]
    public async Task GivenLoggedInAs(string username, string password)
    {
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { Username = username, Password = password });
        if (loginResponse.IsSuccessStatusCode)
        {
            var loginBody = await loginResponse.Content.ReadAsStringAsync();
            var token = JObject.Parse(loginBody)["token"]?.ToString();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
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

    [When(@"adding a product with name ""(.*)"" and quantity ""(.*)""")]
    public async Task WhenAddingAProductWithQuantity(string name, string quantity)
    {
        //Creates the product
        var payload = new { Name = name, Quantity = int.Parse(quantity) };
        _response = await _client.PostAsJsonAsync("/api/products", payload);
    }


    [When(@"decrements ""(.*)"" units of that product")]
    public async Task WhenDecrementsStock(string delta)
    {
        _response = await _client.PostAsync($"/api/products/{_productId}/decrement-stock/{delta}", null);
    }

    [When(@"decrementing ""(.*)"" units of product with id ""(.*)""")]
    public async Task WhenDecrementingStockOfProductID(string delta, string productId)
    {
        _response = await _client.PostAsync($"/api/products/{productId}/decrement-stock/{delta}", null);
    }

    [When(@"incrementing ""(.*)"" units of product with id ""(.*)""")]
    public async Task WhenIncrementingStockOfProductID(string delta, string productId)
    {
        _response = await _client.PostAsync($"/api/products/{productId}/increment-stock/{delta}", null);
    }

    [When(@"increments ""(.*)"" units of that product")]
    public async Task WhenIncrementsStock(string delta)
    {
        _response = await _client.PostAsync($"/api/products/{_productId}/increment-stock/{delta}", null);
    }

    [Then(@"the answer is ""(.*)""")]
    public async Task ThenTheAnswerIs(string expectedStatus)
    {
        Assert.Equal(expectedStatus, _response!.StatusCode.ToString());
    }

    [Then(@"product stock becomes ""(.*)""")]
    public async Task ThenTheStockIs(string expectedStock)
    {
        var product = await _response!.Content.ReadFromJsonAsync<ProductDTO>();
        Assert.Equal(int.Parse(expectedStock), product!.Quantity);
    }

    [When(@"searching for a min quantity of ""(.*)"" and max quantity of ""(.*)""")]
    public async Task WhenSearchingForStockLevels(string min, string max)
    {
        _response = await _client.GetAsync($"api/products/stock-level?min={min}&max={max}");
    }

    [Then(@"has ""(.*)"" results")]
    public async Task TheAmountOfResultsIs(string expectedResults)
    {
        var product = await _response!.Content.ReadFromJsonAsync<IEnumerable<ProductDTO>>();
        Assert.Equal(int.Parse(expectedResults), product == null ? 0 : product.Count());
    }

}