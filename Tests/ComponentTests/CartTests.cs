using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tests.Shared;
using Veli;
using Xunit.Priority;

namespace Tests.ComponentTests;

[Collection("VeliClientCollection")]
[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class CartTests(VeliClientFixture fixture)
{
	private readonly VeliClient _client = fixture.Client;
	
	[Fact, Priority(1)]
	public async Task UserCanAddItem()
	{
		string searchEndpoint = Helpers.MakeSearchPageUri("Samsung");
		
		var response = await _client.GetAsync(searchEndpoint);
		
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		
		ProductObject product = Helpers.GetProduct(JObject.Parse(response.Content));
		
		var dummyProduct = new
		{
			product = product.Id.ToString()
		};
		
		response = await _client.PostAsync(Endpoints.Cart, dummyProduct);
		
		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		
		response = await _client.GetAsync(Endpoints.Cart);
		
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	
		var cartJson = JObject.Parse(response.Content);
	
		Assert.NotNull(cartJson["data"]);
		
		
	}
	
	[Fact, Priority(2)]
	public async Task UserCantSetItemAmountToNegativeValue()
	{
		const string expectedError = "Ensure this value is greater than or equal to 1.";
		
		var response = await _client.GetAsync(Endpoints.Cart);
	
		var cartJson = JObject.Parse(response.Content);
	
		Assert.NotNull(cartJson["data"]);
		
		ProductObject product = Helpers.GetProductFromCart(JObject.Parse(response.Content));
		
		var cartObject = Helpers.GetCartObject(cartJson, product.Id);
		
		var quantity = new
		{
			quantity = -1 
		};
		
		string productEndpoint = Endpoints.Cart + cartObject.ObjectId;
		
		response = await _client.PutAsync( productEndpoint, quantity);
		
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		
		var responseData = JsonConvert.DeserializeObject<JObject>(response.Content);
		var qValue = responseData["quantity"]?.First?.ToString();
		
		Assert.Equal(expectedError, qValue);
	}
	
	[Fact, Priority(3)]
	public async Task UserCantIncreaseItemAmountFurtherThanStock()
	{
		
		const string expectedError = "ProductInsufficientQuantity";
		
		var response = await _client.GetAsync(Endpoints.Cart);
	
		var cartJson = JObject.Parse(response.Content);
	
		Assert.NotNull(cartJson["data"]);
		
		ProductObject product = Helpers.GetProductFromCart(JObject.Parse(response.Content));
		
		var cartObject = Helpers.GetCartObject(cartJson, product.Id);
		
		var quantity = new
		{
			quantity = int.MaxValue
		};
		
		string productEndpoint = Endpoints.Cart + cartObject.ObjectId;
		
		response = await _client.PutAsync( productEndpoint, quantity);
		
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		
		var responseData = JsonConvert.DeserializeObject<JObject>(response.Content);
		var qValue = responseData["code"].ToString();
		
		Assert.Equal(expectedError, qValue);
		
	}
	
}