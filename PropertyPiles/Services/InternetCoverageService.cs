using System.Text;
using System.Text.Json;
using PropertyPiles.Types;
using PropertyPiles.Utils;

namespace PropertyPiles.Services;

public class InternetCoverageService : DataService {
	private readonly string? _apiUrl = Environment.GetEnvironmentVariable("NBN_BASE_URL");

	public InternetCoverageService() {
		if (!this.CanRequestCoverageData()) {
			Logger.Info("Skipping Internet Coverage Service initialization because the API URL is not set.");
		}
	}

	private bool CanRequestCoverageData() {
		if (string.IsNullOrEmpty(_apiUrl)) {
			return false;
		}

		return true;
	}

	public async Task<NbnCoverageResponse?> GetCoverageForProperty(PropertyRecord property) {
		if (!this.CanRequestCoverageData()) return null;
		
		var cached = this.GetCachedCoverageData(property.Id);
		if (cached != null) {
			return cached;
		}
		
		if (property.Data == null || property.GetAddressData() == null) {
			throw new ArgumentNullException(nameof(property.Data), $"Property data is not sufficient to check NBN coverage for {property.GetShortAddress()}");
		}
	
		return await this.FetchCoverageForProperty(property);
	}

	private NbnCoverageResponse? GetCachedCoverageData(string propertyId) {
		string filePath = Path.Combine(this.CacheDir, $"{propertyId}_nbn.json");
		if (!File.Exists(filePath)) {
			return null;
		}

		try {
			string jsonString = File.ReadAllText(filePath);
			using (JsonDocument doc = JsonDocument.Parse(jsonString)) {
				Logger.Info($"Found cached internet coverage data for property {propertyId}");
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

	/// <summary>
	/// Initial request to the ISP API to get the ID they use for this property,
	/// to be used for the second query that gets the actual coverage data.
	/// </summary>
	/// <param name="property"></param>
	/// <returns></returns>
	private async Task<string?> FetchPropertyIdForQuery(PropertyRecord property) {
		try {
			var request = new HttpRequestMessage {
				Method = HttpMethod.Get,
				RequestUri = new Uri(this._apiUrl! + "/api/signup/address/search/?address=" + property.GetFormattedAddress(withPostcode: true).Replace(",", "").Replace(" ", "+"))
			};
			
			using (var response = await this.Client.SendAsync(request)) {
				response.EnsureSuccessStatusCode();
				string body = await response.Content.ReadAsStringAsync();
				var json = JsonDocument.Parse(body);
				var results = json.Deserialize<Dictionary<string, string>>();
				
				if (results == null) {
					return null;
				}
				
				return this.FindKeyForAddress(results, property);
			}
		}
		catch (Exception ex) {
			Logger.Error(ex.Message);
			return null;
		}
	}

	private string? FindKeyForAddress(Dictionary<string, string> results, PropertyRecord property) {
		foreach (var item in results) {
			if (item.Value.Equals(property.GetFormattedAddress(true, true, true), StringComparison.OrdinalIgnoreCase)) {
				return item.Key;
			}
		}

		return null;
	}


	/// <summary>
	/// Query an ISP's API to get NBN/Opticomm coverage data for the property.
	/// </summary>
	/// <param name="property"></param>
	/// <returns></returns>
	private async Task<NbnCoverageResponse?> FetchCoverageForProperty(PropertyRecord property) {
		string? ispPropertyId = await this.FetchPropertyIdForQuery(property);
		
		if (string.IsNullOrEmpty(ispPropertyId)) {
			Logger.Warning($"No property ID for ISP query found for {property.GetShortAddress()}");
			return null;
		}
		
		try {
			var request = new HttpRequestMessage {
				Method = HttpMethod.Get,
				RequestUri = new Uri(this._apiUrl! + $"/api/signup/service-qualification/{ispPropertyId}"),
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