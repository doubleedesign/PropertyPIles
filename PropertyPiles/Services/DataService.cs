using System.Text.Json;
using PropertyPiles.Types;

namespace PropertyPiles.Services;

internal class DataService {
	private readonly HttpClient _client = new HttpClient();
	private readonly string _apiKey = Environment.GetEnvironmentVariable("REALTY_API_KEY") ?? "";
	private readonly string _baseUrl = Environment.GetEnvironmentVariable("REALTY_API_BASE_URL") ?? "";
	
	public DataService() {
		if(String.IsNullOrEmpty(this._apiKey)) {
			throw new Exception("REALTY_API_KEY environment variable is not set.");
		}
		
		if (String.IsNullOrEmpty(this._baseUrl)) {
			throw new Exception("REALTY_API_BASE_URL environment variable is not set.");
		}
	}

	/// <summary>
	/// Fetch live property data from the API.
	/// </summary>
	/// <param name="url">The API GET request URL.</param>
	/// <returns>The relevant fields from the API response.</returns>
	/// <exception cref="HttpRequestException"></exception>
	private async Task<PropertyDataResponse?> GetProperty(string url) {
		var request = new HttpRequestMessage(HttpMethod.Get, url);
		request.Headers.Add("x-realtyapi-key", this._apiKey);
		var response = await this._client.SendAsync(request);
		
		if (!response.IsSuccessStatusCode) {
			throw new HttpRequestException($"Failed to fetch property data");
		}
		
		var body = await response.Content.ReadAsStringAsync();
		var json = JsonDocument.Parse(body);
		var details = json.RootElement.GetProperty("detail");
		
		return details.Deserialize<PropertyDataResponse>();
	}
	
	/// <summary>
	/// Fetch live property data by the ID of the listing.
	/// </summary>
	/// <param name="id"></param>
	/// <exception cref="HttpRequestException">Thrown when the third-party API returns a non-success status code.</exception>
	/// <returns>The relevant fields from the API response.</returns>
	public async Task<PropertyDataResponse?> GetPropertyById(string id) {
		var url = $"{this._baseUrl}/details/byid?id={id}";

		try {
			return await this.GetProperty(url);
		}
		catch (HttpRequestException ex) {
			throw new HttpRequestException($"Failed to fetch property data for property ID {id}");
		}
	}
	
	/// <summary>
	/// Fetch live property data by the URL path of the listing.
	/// </summary>
	/// <param name="path"></param>
	/// <exception cref="HttpRequestException">Thrown when the third-party API returns a non-success status code.</exception>
	/// <returns>The relevant fields from the API response.</returns>
	public async Task<PropertyDataResponse?> GetPropertyByPath(string path) {
		var url = $"{this._baseUrl}/details/byurl?url={path}";
		
		try {
			return await this.GetProperty(url);
		}
		catch (HttpRequestException ex) {
			throw new HttpRequestException($"Failed to fetch property data for property '{path}'");
		}
	}
}