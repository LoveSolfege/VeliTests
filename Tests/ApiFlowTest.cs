﻿using System.Diagnostics.CodeAnalysis;
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
public class ApiFlowTest(VeliClientFixture fixture)
{
	private const string Query = "Sennheiser";
	private readonly VeliClient _client = fixture.Client;

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
		var category = GetCategoryParams(JObject.Parse(response.Content)); //get random available category from search result
		
		string categoryUri = MakeCategoryUri(category);
		
		response = await _client.GetAsync(categoryUri); //go to the category
		
		var filters = GetFilterOptions(JObject.Parse(response.Content)); //get random available filter for the category
		
		string filterUri = MakeFilterUri(categoryUri, filters);
		
		response = await _client.GetAsync(filterUri); //Request filtered products
		
		Assert.Equal(HttpStatusCode.OK, response.StatusCode); //ensure filter is valid
		
		
		
		//User sorts products by price descending
		
		string descUri = MakeDescendingSortUri(filterUri, Query); //Make URI for sorting by descending price
		
		response = await _client.GetAsync(descUri); //Request sorted products

		var prices = GetAllPrices(JObject.Parse(response.Content)); //Get prices of all products on the page
		
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
			ProductObject product = GetProduct(JObject.Parse(response.Content));
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
		var cartObject = GetCartObject(cartJson, id); //Get cart object for needed product
		
		var quantity = new
		{
			quantity = Random.Shared.NextInt64(1, cartObject.Quantity) //random amount for product
		};
		
		string productEndpoint = Endpoints.Cart + cartObject.ObjectId; // create cart object endpoint
		
		response = await _client.PutAsync( productEndpoint, quantity); // update product quantity in cart
		
		Assert.Equal(HttpStatusCode.OK, response.StatusCode); //Ensure change is successful
		
		
		
		//User verifies that product details are correct
		response = await _client.GetAsync(Endpoints.Cart); // get updated cart
		
		cartObject = GetCartObject(JObject.Parse(response.Content), id); //get update cart object
		
		Assert.Equal(quantity.quantity, cartObject.Quantity ); //verify that quantity is updated
		
	}

	
	
	//Helper methods
	CartObject GetCartObject(JObject cart, long id)
	{
		JArray cartAsArray = (JArray)cart["data"];
		
		var cartObject = cartAsArray
			.Where(item => (long)item["product"]["id"] == id)
			.Select(item => new CartObject(
				ObjectId: (long)item["id"],
				Quantity: (long)item["quantity"]
			))
			.FirstOrDefault();
		
		return cartObject;
	}

	ProductObject GetProduct(JObject searchResult)
	{
		JArray resultAsArray = (JArray)searchResult["pageProps"]["data"]["products"];
    
		Console.WriteLine(resultAsArray);
		
		var searchObject = resultAsArray
			.Select(item => new ProductObject(
				Id: (long)item["id"],
				Sku: (string)item["sku"]
			))
			.FirstOrDefault();

		Console.WriteLine(searchObject.Id);
    
		return searchObject;
	}
	
	CategoryParams GetCategoryParams(JObject searchResult)
	{
		JArray resultAsArray = (JArray)searchResult["pageProps"]["data"]["categories"];
		
		var index = RndIndex(resultAsArray);
		
		var randomObject = resultAsArray[index];
			
		var categoryParams = new CategoryParams(
				CategoryId: (long)randomObject["id"],
				Headline: (string)randomObject["headline"],
				FullSlug: (string)randomObject["full_slug"]
		);
		
		return categoryParams;
	}
	
	FilterOptions GetFilterOptions(JObject searchResult)
	{
		JArray resultAsArray = (JArray)searchResult["pageProps"]["data"]["filters"];
		
		var index = RndIndex(resultAsArray);
		
		var randomObject = resultAsArray[index];

		var filterOptions = ((JArray)randomObject["filter_options"])
			.Select(option => new FilterOption(
				OptionId: (long)option["id"],
				Headline: (string)option["headline"]
			))
			.ToList();
		
		var filterIndex = RndIndex(filterOptions);
		var filterOption = filterOptions[filterIndex];
		
		var filter = new FilterOptions(
			FilterId: (long)randomObject["id"],
			Headline: (string)randomObject["headline"],
			Option: filterOption
		);
		
		return filter;
	}
	
	List<Stock> GetAllPrices(JObject searchResult)
	{
		JArray resultAsArray = (JArray)searchResult["pageProps"]["data"]["products"];
		
		if (resultAsArray.Count != 0)
		{
			
			return resultAsArray.Select(item => new Stock(
				StartPrice: (decimal)item["stock"]["start_price"],
				Price: (decimal)item["stock"]["price"]
			)).ToList();
		}
		
		return [];
	}

	
	string[] CutOnDashes(string input)
	{
		return input.Split('/');
	}

	string MakeCategoryUri(CategoryParams category)
	{
		
		string[] slugs = CutOnDashes(category.FullSlug);
		
		return Endpoints.Category + "/" + //create category URI 
		       category.FullSlug + "/" +
		       category.CategoryId.ToString() + ".json" +
		       "?q=" + Query +
		       "&type" + slugs[0] + 
		       "&type=" + slugs[1] +
		       "&type=" + slugs[2] +
		       "&type=" + category.CategoryId.ToString();
	}

	string MakeFilterUri(String categoryUri, FilterOptions filters)
	{
		return categoryUri + "&filter_options=" + filters.Option.OptionId.ToString(); 
	}

	string MakeDescendingSortUri(String normalUri, string query)
	{
		int index = normalUri.IndexOf(query);
		const string desc = "&ordering=-price";
		return normalUri.Substring(0, index + query.Length) + desc + normalUri.Substring(index + query.Length);
	}
	
	int RndIndex<T>(IEnumerable<T> array){
		return Random.Shared.Next(0, array.Count());
	}
	
	
	//helper data structures
	private record CartObject(long ObjectId, long Quantity);
	private record ProductObject(long Id, string Sku);
	private record CategoryParams(long CategoryId, string Headline, string FullSlug);
	private record FilterOptions(long FilterId, string Headline, FilterOption Option);
	private record FilterOption(long OptionId, string Headline);
	private record Stock(decimal StartPrice, decimal Price);
}