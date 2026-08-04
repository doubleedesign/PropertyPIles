using PropertyPiles.Extensions;
using System.Text.Json;
using PropertyPiles.Types;

namespace PropertyPiles.Services;

public class FileService {
	private List<SavedItem>? _savedItems;
	
	public FileService() {
	}
	
	public async Task LoadFile() {
		var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
		string? json = isDev ? this.GetLocalFileContents() : await this.GetRemoteFileContents();
		
		if(json is null || String.IsNullOrEmpty(json)) {
			throw new FileLoadException("Failed to load data.json from storage service, or the file is empty.");
		}
		
		this._savedItems = JsonSerializer.Deserialize<List<SavedItem>>(json, new JsonSerializerOptions { AllowTrailingCommas = true });
	}

	private string GetLocalFileContents() {
		using (StreamReader r = new StreamReader("data.json")) {
			return r.ReadToEnd();
		}
	}

	private async Task<string> GetRemoteFileContents() {
		return await new BlobService().DownloadBlobContents() ?? "";
	}
	
	public List<SavedItem>? GetItemsFromFile() {
		return this._savedItems;
	}

	public void LogFileContents() {
		if (this._savedItems == null) {
			return;
		}

		Console.WriteLine("\nFile contents:");
		this._savedItems.LogToConsole();
	}
}