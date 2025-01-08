using System.Net;
using RestSharp;

namespace Veli; 

public class VeliClient : IDisposable
{
	private readonly RestClient _client;
	private readonly CookieContainer _cookieContainer;
	
	public VeliClient()
	{
		_cookieContainer = new CookieContainer();
		_client = new RestClient(new RestClientOptions
		{
			BaseUrl = new Uri("https://veli.store/"),
			CookieContainer = _cookieContainer
		});
	}

	public async Task<RestResponse> GetAsync(string endpoint)
	{
		var request = new RestRequest(endpoint, Method.Get);
		request.AddHeader("Accept", "application/json");
		request.AddHeader("Accept-Language", "en");
		var response = await _client.ExecuteAsync(request);
		return response;
	}

	public async Task<RestResponse> PostAsync(string endpoint, object body)
	{
		var request = new RestRequest(endpoint, Method.Post);
		request.AddHeader("Accept", "application/json");
		request.AddHeader("Accept-Language", "en");
		request.AddJsonBody(body);
		var response = await _client.ExecuteAsync(request);
		return response;
	}

	public async Task<RestResponse> PutAsync(string endpoint, object body)
	{
		var request = new RestRequest(endpoint, Method.Put);
		request.AddHeader("Accept", "application/json");
		request.AddHeader("Accept-Language", "en");
		request.AddJsonBody(body);
		var response = await _client.ExecuteAsync(request);
		return response;
	}
	
	public string GetClientId()
	{
		var cookies = _cookieContainer.GetCookies(new Uri("https://veli.store"));
		var clientIdCookie = cookies["client_id"];

		return clientIdCookie!.Value;
	}

	public void Dispose()
	{
		_client.Dispose();
	}
}
