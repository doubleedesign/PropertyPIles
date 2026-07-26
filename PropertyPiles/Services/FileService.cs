using Humanizer;

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
		Console.WriteLine(string.Concat(Enumerable.Repeat("==========", 15)));
		Console.WriteLine("Id".PadRight(16) + "Priority".PadRight(12) + "Address".PadRight(40) + "Notes".PadRight(30));
		Console.WriteLine(string.Concat(Enumerable.Repeat("----------", 15)));
		
		foreach (SavedItem item in this._savedItems) {
			Console.Write($"{item.Id}".PadRight(16));
			Console.Write(item.IsPriority ? "Yes".PadRight(12) : "-".PadRight(12));
			Console.Write(item.GetShortAddress().PadRight(40));

			var combinedNotes = (item.Notes ?? []).ToList().Concat((item.DismissedReasons ?? []).ToList());
			Console.Write(string.Join(", ", combinedNotes).Transform(To.LowerCase).Transform(To.SentenceCase).PadRight(30));

			Console.WriteLine();
		}

		Console.WriteLine(string.Concat(Enumerable.Repeat("==========", 15)));
	}
}