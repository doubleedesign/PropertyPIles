namespace PropertyPile.Services;

public class DataService {
	private readonly string _apiKey = Environment.GetEnvironmentVariable("REALTY_API_KEY") ?? "";
	private readonly HttpClient _httpClient = new HttpClient();
	private readonly string _baseUrl = Environment.GetEnvironmentVariable("REALTY_API_BASE_URL") ?? "";
	
	public DataService() {
		if(String.IsNullOrEmpty(this._apiKey)) {
			throw new Exception("REALTY_API_KEY environment variable is not set.");
		}
		
		if (String.IsNullOrEmpty(this._baseUrl)) {
			throw new Exception("REALTY_API_BASE_URL environment variable is not set.");
		}
	}
	
	public dynamic GetPropertyById(int id) {
		var url = $"{this._baseUrl}/details/byid?id={id}";
		// var request = new HttpRequestMessage(HttpMethod.Get, url);
		// request.Headers.Add("x-realtyapi-key", apiKey);
		// var response = await client.SendAsync(request);
		// body = await response.Content.ReadAsStringAsync();
		//Console.WriteLine(body); 

		return new { };
	}
}