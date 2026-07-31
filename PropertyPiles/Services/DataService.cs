using System.Text.Json;
using System.Text.Json.Nodes;
using PropertyPiles.Utils;

namespace PropertyPiles.Services;

public abstract class DataService {
	protected readonly HttpClient Client = new();
	protected readonly string CacheDir;

	protected DataService() {
		var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
		Directory.CreateDirectory(Path.Combine(projectRoot, "cache"));
		this.CacheDir = Path.Combine(projectRoot, "cache");
	}
	
	
	/// <summary>
	/// Save the API response as a JSON file.
	/// </summary>
	/// <param name="response"></param>
	protected void CacheResponse(JsonDocument response) {
		try {
			var propertyId = response.RootElement.GetProperty("detail").GetProperty("id").ToString();
			string outputPath = Path.Combine(this.CacheDir, $"{propertyId}.json");

			// Convert to mutable JsonObject and add timestamp
			string rawJson = response.RootElement.GetRawText();
			JsonObject jsonObject = JsonSerializer.Deserialize<JsonObject>(rawJson)!;
			jsonObject.Add("timestamp", JsonValue.Create(DateTimeOffset.UtcNow.ToUnixTimeSeconds()));

			// Write to file
			var options = new JsonWriterOptions { Indented = true };
			using FileStream fileStream = File.Create(outputPath);
			using Utf8JsonWriter writer = new Utf8JsonWriter(fileStream, options);
			jsonObject.WriteTo(writer);

			Logger.Info($"Cached property data for property {propertyId}");
		}
		catch (Exception ex) {
			Logger.Error(ex.Message);
		}
	}
}