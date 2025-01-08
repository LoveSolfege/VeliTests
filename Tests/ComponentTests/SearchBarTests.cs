using System.Diagnostics.CodeAnalysis;
using Tests.Shared;

namespace Tests.ComponentTests;

[SuppressMessage("ReSharper", "Xunit.XunitTestWithConsoleOutput")]
[Collection("VeliClientCollection")]
public class SearchBarTests(VeliClientFixture fixture)
{/*
	private readonly VeliClient _client = fixture.Client;
	private readonly string TestQuery1 = "Sennheiser";
	private readonly string TestQuery2 = "Sennheiser";
	private readonly string TestQuery3 = "Sennheiser";
	
	[Fact]
	public async Task CanSearch()
	{
		string url = $"{Endpoints.SuggestionSearch}?q={TestQuery1}";
		var response = await _client.GetAsync(url);
		Console.WriteLine(response.StatusCode);


		if (response.StatusCode == HttpStatusCode.OK)
		{
			var jsonResponse = JsonConvert.DeserializeObject<JArray>(response.Content!);
			Console.WriteLine(jsonResponse!.ToString());

			var sku = jsonResponse[0].Value<string>("sku");

			if (!string.IsNullOrEmpty(sku))
				Console.WriteLine($" Extracted SKU: {sku}");
			else
				Console.WriteLine("SKU not found in response");

		}
		else if (response.StatusCode == HttpStatusCode.BadRequest)
		{
			string expectedError = "მნიშვნელობას უნდა ჰქონდეს სულ ცოტა 2 სიმბოლო (მას აქვს 1).";
			var responseData = JsonConvert.DeserializeObject<JObject>(response.Content);
			var qValue = responseData["q"]?.First?.ToString();

			if (qValue != null)
			{
				Assert.Equal(expectedError, qValue);
			}
			else
			{
				Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
			}
		}
	}
*/
}