using System.Diagnostics.CodeAnalysis;
using System.Net;
using Newtonsoft.Json.Linq;
using Tests.Shared;
using Veli;

#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.

namespace Tests;

[SuppressMessage("ReSharper", "Xunit.XunitTestWithConsoleOutput")]
[Collection("VeliClientCollection")]
public class ApiFlowTest(VeliClientFixture fixture)//shared client for all tests
{
	private const string Query = "Sennheiser";
	private readonly VeliClient _client = fixture.Client; //using shared client

	[Fact]
	public async Task Flow()
	{
		
		//Creating visitor client ID
		var response = await _client.PostAsync(Endpoints.Visitor, String.Empty); //create client_id 
		
		Assert.Equal(HttpStatusCode.Created, response.StatusCode); //ensure client_id is created
		
		var clientId = _client.GetClientId();
		
		Assert.NotNull(clientId); //ensure client_id is accessible in code
	
		
		
		
		//User starts to search a product
		string searchEndpoint = $"{Endpoints.SuggestionSearch}?q={Query}"; //create search query endpoint
		
		response = await _client.GetAsync(searchEndpoint); //search for item
		
		var skus = JArray.Parse(response.Content); //get response array
		
		Assert.NotEmpty(skus); //ensure array is not null
		
			
			
		//User finds specific page of products
		response = await _client.GetAsync($"{Endpoints.SearchPageResult}?q={Query}"); //get all products
		
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		
		
		
		//User filters the products
		var category = Helpers.GetCategoryParams(JObject.Parse(response.Content)); //get random available category from search result
		
		string categoryUri = Helpers.MakeCategoryUri(category, Query);
		
		response = await _client.GetAsync(categoryUri); //go to the category
		
		var filters = Helpers.GetFilterOptions(JObject.Parse(response.Content)); //get random available filter for the category
		
		string filterUri = Helpers.MakeFilterUri(categoryUri, filters);
		
		response = await _client.GetAsync(filterUri); //Request filtered products
		
		Assert.Equal(HttpStatusCode.OK, response.StatusCode); //ensure filter is valid
		
		
		
		//User sorts products by price descending
		
		string descUri = Helpers.MakeDescendingSortUri(filterUri, Query); //Make URI for sorting by descending price
		
		response = await _client.GetAsync(descUri); //Request sorted products

		var prices = Helpers.GetAllPrices(JObject.Parse(response.Content)); //Get prices of all products on the page
		
		if (prices.Count > 0) //if any products present
		{
			var sortedPrices = prices.OrderByDescending(x => x.Price);
			Assert.Equal(sortedPrices, prices);//ensure prices are sorted
		}
		else //if all products were filtered out
		{
			Console.WriteLine("ALL PRODUCTS WERE FILTERED OUT");
			Assert.Equal(HttpStatusCode.OK, response.StatusCode); //check if request was successful
			Assert.Empty(prices); //make sure no products are present
		}
		
		
		
		//User tries to add product to wishlist
		//can't be done as visitor
		
		
		
		//User tries to add product to cart
		long id = 168127;//prefixed valid id
		
		if (prices.Count > 0) //if any products present
		{
			ProductObject product = Helpers.GetProduct(JObject.Parse(response.Content));
			id = product.Id;
			var dummyProduct = new
			{
				product = id.ToString()
			};
			response = await _client.PostAsync(Endpoints.Cart, dummyProduct); //add most expensive item to cart
		}
		else // if all products are filtered out
		{
			var dummyProduct = new
			{
				product = id.ToString() //use prefixed id
			};
			
			response = await _client.PostAsync(Endpoints.Cart, dummyProduct); //add specific item to cart (for the test's sake)
		}
		
		Assert.Equal(HttpStatusCode.Created, response.StatusCode); //ensure item was added to cart
	
		response = await _client.GetAsync(Endpoints.Cart); //get whole cart
	
		var cartJson = JObject.Parse(response.Content);
	
		Assert.NotNull(cartJson["data"]); //ensure cart is not empty
			
		
		
		//User tries to increase product amount in cart based on quantity value
		var cartObject = Helpers.GetCartObject(cartJson, id); //Get cart object for needed product
		
		var quantity = new
		{
			quantity = Random.Shared.NextInt64(1, cartObject.Quantity) //random amount for product
		};
		
		string productEndpoint = Endpoints.Cart + cartObject.ObjectId; // create cart object endpoint
		
		response = await _client.PutAsync( productEndpoint, quantity); // update product quantity in cart
		
		Assert.Equal(HttpStatusCode.OK, response.StatusCode); //Ensure change is successful
		
		
		
		//User verifies that product details are correct
		response = await _client.GetAsync(Endpoints.Cart); // get updated cart
		
		cartObject = Helpers.GetCartObject(JObject.Parse(response.Content), id); //get update cart object
		
		Assert.Equal(quantity.quantity, cartObject.Quantity ); //verify that quantity is updated
		
	}
	
}