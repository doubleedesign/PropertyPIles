using System.Text;
using System.Text.Json;
using PropertyPiles.Types;
using PropertyPiles.Utils;

namespace PropertyPiles.Services;

public class NbnCoverageService : DataService {
	private readonly string? _apiUrl = Environment.GetEnvironmentVariable("NBN_BASE_URL");
	private readonly string? _ispName = Environment.GetEnvironmentVariable("NBN_ISP_NAME");

	public NbnCoverageService() {
		if (!this.CanRequestNbnData()) {
			Logger.Info("Skipping NBN Coverage Service initialization because the API URL or ISP name is not set.");
		}
	}

	private bool CanRequestNbnData() {
		if (string.IsNullOrEmpty(_apiUrl)) {
			return false;
		}

		if (string.IsNullOrEmpty(_ispName)) {
			return false;
		}

		return true;
	}

	public async Task<NbnCoverageResponse?> GetCoverageForProperty(PropertyRecord property) {
		if (!this.CanRequestNbnData()) return null;
		
		var cached = this.GetCachedNbnData(property.Id);
		if (cached != null) {
			return cached;
		}
		
		if (property.Data == null || property.GetAddressData() == null) {
			throw new ArgumentNullException(nameof(property.Data), $"Property data is not sufficient to check NBN coverage for {property.GetShortAddress()}");
		}
	
		return await this.FetchCoverageForProperty(property);
	}

	private NbnCoverageResponse? GetCachedNbnData(string propertyId) {
		string filePath = Path.Combine(this.CacheDir, $"{propertyId}_nbn.json");
		if (!File.Exists(filePath)) {
			return null;
		}

		try {
			string jsonString = File.ReadAllText(filePath);
			using (JsonDocument doc = JsonDocument.Parse(jsonString)) {
				Logger.Info($"Found cached NBN coverage data for property {propertyId}");
				return this.ConvertJsonResponseDetail(doc);
			}
		}
		catch (Exception ex) {
			Logger.Error(ex.Message);
			return null;
		}
	}
	
	private NbnCoverageResponse? ConvertJsonResponseDetail(JsonDocument json) {
		var details = json.RootElement;
		
		return details.Deserialize<NbnCoverageResponse>();
	}

	private async Task<NbnCoverageResponse?> FetchCoverageForProperty(PropertyRecord property) {
		var requestBody = new Dictionary<string, object> {
			{ "addressDetails", property.GetAddressData()! },
			{ "brand", this._ispName ?? "" }
		};

		var requestBodyString = new StringContent(
			JsonSerializer.Serialize(requestBody),
			Encoding.UTF8,
			"application/json"
		);

		try {
			var request = new HttpRequestMessage {
				Method = HttpMethod.Post,
				RequestUri = new Uri(this._apiUrl!),
				Content = requestBodyString
			};
			using (var response = await this.Client.SendAsync(request)) {
				response.EnsureSuccessStatusCode();
				var body = await response.Content.ReadAsStringAsync();
				var json = JsonDocument.Parse(body);
				this.CacheResponse(json, $"{property.Id}_nbn");

				return json.Deserialize<NbnCoverageResponse>();
			}
		}
		catch (Exception ex) {
			Logger.Error(ex.Message);
			return null;
		}
	}
}