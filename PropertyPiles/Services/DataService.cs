using System.Text.Json;
using System.Text.Json.Nodes;
using PropertyPiles.Types;

namespace PropertyPiles.Services;

internal class DataService {
	private readonly HttpClient _client = new HttpClient();
	private readonly string _apiKey = Environment.GetEnvironmentVariable("REALTY_API_KEY") ?? "";
	private readonly string _baseUrl = Environment.GetEnvironmentVariable("REALTY_API_BASE_URL") ?? "";
	private readonly string _cacheDir;
	
	public DataService() {
		var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
		Directory.CreateDirectory(Path.Combine(projectRoot, "cache"));
		this._cacheDir = Path.Combine(projectRoot, "cache");
		
		if(String.IsNullOrEmpty(this._apiKey)) {
			throw new Exception("REALTY_API_KEY environment variable is not set.");
		}
		
		if (String.IsNullOrEmpty(this._baseUrl)) {
			throw new Exception("REALTY_API_BASE_URL environment variable is not set.");
		}
	}
	
	/// <summary>
	/// Fetch live property data by the ID of the listing.
	/// </summary>
	/// <param name="id"></param>
	/// <exception cref="HttpRequestException">Thrown when the third-party API returns a non-success status code.</exception>
	/// <returns>The relevant fields from the API response.</returns>
	public async Task<PropertyDataResponse?> GetPropertyById(string id) {
		var cached =  this.GetCachedPropertyById(id);
		if (cached != null) {
			return cached;
		}
		
		var url = $"{this._baseUrl}/details/byid?id={id}";
		return await this.FetchProperty(url);
	}

	private PropertyDataResponse? GetCachedPropertyById(string id) {
		string filePath = Path.Combine(this._cacheDir, $"{id}.json");
		if (!File.Exists(filePath)) {
			return null;
		}

		string jsonString = File.ReadAllText(filePath);
		using (JsonDocument doc = JsonDocument.Parse(jsonString)) {
			long timestamp = doc.RootElement.GetProperty("timestamp").GetInt64();
			long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			long timeAgo = now - timestamp;
			// Return the data if less than or equal to an hour old
			if (timeAgo < 3600) {
				return doc.Deserialize<PropertyDataResponse>();
			}
		}

		return null;
	}
	
	/// <summary>
	/// Fetch live property data by the URL path of the listing.
	/// </summary>
	/// <param name="path"></param>
	/// <exception cref="HttpRequestException">Thrown when the third-party API returns a non-success status code.</exception>
	/// <returns>The relevant fields from the API response.</returns>
	public async Task<PropertyDataResponse?> GetPropertyByPath(string path) {
		var cached =  this.GetCachedPropertyByPath(path);
		if (cached != null) {
			return cached;
		}
		
		var url = $"{this._baseUrl}/details/byurl?url={path}";
		return await this.FetchProperty(url);
	}

	
	private PropertyDataResponse? GetCachedPropertyByPath(string path) {
		var assumedId = path.Split("-").Last();
		var cached = this.GetCachedPropertyById(assumedId);
		if (cached == null) {
			return null;
		}
		
		if (cached.ListingUrl == null) {
			return null;
		}

		string cachedPath = cached.ListingUrl.Split("/").Last();
		if (path == cachedPath) {
			return cached;
		}
		
		return null;
	}
	
	
	/// <summary>
	/// Fetch live property data from the API.
	/// </summary>
	/// <param name="url">The API GET request URL.</param>
	/// <returns>The relevant fields from the API response.</returns>
	/// <exception cref="HttpRequestException"></exception>
	private async Task<PropertyDataResponse?> FetchProperty(string url) {
		var request = new HttpRequestMessage(HttpMethod.Get, url);
		request.Headers.Add("x-realtyapi-key", this._apiKey);
		var response = await this._client.SendAsync(request);
		
		if (!response.IsSuccessStatusCode) {
			throw new HttpRequestException($"Failed to fetch property data from API. Status code: {response.StatusCode}");
		}
		
		var body = await response.Content.ReadAsStringAsync();
		var json = JsonDocument.Parse(body);
		var details = json.RootElement.GetProperty("detail");
		
		this.CacheResponse(json);
		return details.Deserialize<PropertyDataResponse>();
	}
	

	/// <summary>
	/// Save the API response as a JSON file.
	/// </summary>
	/// <param name="response"></param>
	private void CacheResponse(JsonDocument response) {
		try {
			var propertyId = response.RootElement.GetProperty("detail").GetProperty("id").ToString();
			string outputPath = Path.Combine(this._cacheDir, $"{propertyId}.json");

			// Convert to mutable JsonObject and add timestamp
			string rawJson = response.RootElement.GetRawText();
			JsonObject jsonObject = JsonSerializer.Deserialize<JsonObject>(rawJson)!;
			jsonObject.Add("timestamp", JsonValue.Create(DateTimeOffset.UtcNow.ToUnixTimeSeconds()));

			// Write to file
			var options = new JsonWriterOptions { Indented = true };
			using FileStream fileStream = File.Create(outputPath);
			using Utf8JsonWriter writer = new Utf8JsonWriter(fileStream, options);
			jsonObject.WriteTo(writer);
		}
		 catch (Exception ex) {
		 	Console.ForegroundColor = ConsoleColor.Red;
		    Console.WriteLine(ex.Message);
		 	Console.ResetColor();
		 }
	}
}