using System.Diagnostics.CodeAnalysis;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tests.Shared;
using Veli;

namespace Tests.ComponentTests;

[SuppressMessage("ReSharper", "Xunit.XunitTestWithConsoleOutput")]
[Collection("VeliClientCollection")]
public class SearchBarTests(VeliClientFixture fixture)
{
	private readonly VeliClient _client = fixture.Client;
	
	private const string ShortQuery = "a";
	private static readonly string EmptyQuery = string.Empty;
	private const string NormalQuery = "Pixel";
	
	[Fact]
	public async Task UserCantSearchWhenQueryTooShort()
	{
		string url = $"{Endpoints.SuggestionSearch}?q={ShortQuery}";
		string expectedError = "Ensure this value has at least 2 characters (it has 1).";
		
		var response = await _client.GetAsync(url);
		
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		
		var responseData = JsonConvert.DeserializeObject<JObject>(response.Content);
		var qValue = responseData["q"].First.ToString();
		
		Assert.Equal(expectedError, qValue);
	}
	
	[Fact]
	public async Task UserCantSearchWhenQueryEmpty()
	{
		string url = $"{Endpoints.SuggestionSearch}?q={EmptyQuery}";
		string expectedError = "This field is required.";
		
		var response = await _client.GetAsync(url);
		
		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		
		var responseData = JsonConvert.DeserializeObject<JObject>(response.Content);
		var qValue = responseData["q"]?.First?.ToString();
		
		Assert.Equal(expectedError, qValue);
	}
	
	[Fact]
	public async Task UserCanSearch()
	{
		string url = $"{Endpoints.SuggestionSearch}?q={NormalQuery}";
		var response = await _client.GetAsync(url);
		
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		
		var jsonResponse = JsonConvert.DeserializeObject<JArray>(response.Content!);
		var sku = jsonResponse[0].Value<string>("sku");
		
		Assert.NotNull(sku);
	}
	
}