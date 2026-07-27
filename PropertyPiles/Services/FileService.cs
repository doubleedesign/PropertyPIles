using Humanizer;
using PropertyPiles.Extensions;

namespace PropertyPiles.Services;
using Types;
using System.Text.Json;

internal class FileService {
	private List<SavedItem>? _savedItems;
	
	public FileService() {
		this.LoadFile();
	}
	
	private void LoadFile() {
		using (StreamReader r = new StreamReader("data.json")) {  
			string json = r.ReadToEnd();
			this._savedItems = JsonSerializer.Deserialize<List<SavedItem>>(json);

			if (this._savedItems == null) {
				throw new FileLoadException("Failed to load data.json or the file is empty.");
			}
		}
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