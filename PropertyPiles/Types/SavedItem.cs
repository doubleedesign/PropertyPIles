namespace PropertyPiles.Types;
using System.Text.Json.Serialization;
using Humanizer;

public class SavedItem {
	[JsonPropertyName("path")]
	public required string Path { get; set; }

	[JsonPropertyName("notes")]
	public string[]? Notes { get; set; }

	[JsonPropertyName("dismissedReasons")]
	public string[]? DismissedReasons { get; set; }

	[JsonPropertyName("priority")]
	public bool IsPriority { get; set; }

	public string Id => this.Path.Split("-").Last();

	public string GetUrl() {
		return Environment.GetEnvironmentVariable("SOURCE_SITE_BASE_URL") + this.Path;
	}

	public string GetShortAddress(bool withPostcode = false) {
		var pathPieces = this.Path.Split("-").ToList();
		// If the first two pieces are both numbers, join them and add a slash
		if (pathPieces[0].All(char.IsDigit) && pathPieces[1].All(char.IsDigit)) {
			var number = $"{pathPieces[0]}/{pathPieces[1]}";
			pathPieces.Insert(0, number);		
			pathPieces.RemoveAt(1); // removes the first number
			pathPieces.RemoveAt(1); // the second number will have moved up, so this removes that too
		}
		
		// Find the piece containing the postcode, working from the end of the array so we don't get the house number by accident
		var postcodeIndex = pathPieces.FindIndex(piece => piece.Length.Equals(4) && piece.All(char.IsDigit));
		var addressPieces = pathPieces.GetRange(0, postcodeIndex - 1); // -1 removes Vic too

		// Note: To.TitleCase preserves slashes in house numbers, Humanize(LetterCasing.Title) does not
		return string.Join(" ", addressPieces).Transform(To.TitleCase);
	}

	public List<string> GetNotes() {
		if (this.DismissedReasons != null && this.DismissedReasons.Length > 0) {
			return this.DismissedReasons.ToList();
		}

		return this.Notes?.ToList() ?? [];
	}

	public string GetFormattedNotes() {
		if(this.DismissedReasons != null && this.DismissedReasons.Length > 0) {
			return string.Join(", ", this.DismissedReasons).Transform(To.LowerCase).Transform(To.SentenceCase);
		} 
		
		return string.Join(", ", this.Notes ?? []).Transform(To.LowerCase).Transform(To.SentenceCase);
	}
}