using System.Diagnostics.CodeAnalysis;
using PropertyPiles.Services;

namespace PropertyPiles.Types;

public class PropertyRecord : SavedItem {
	private ListingDataService _api = new();
	internal PropertyDataResponse? Data;
	
	// A saved item is usually expected to exist before attempting to create a property record,
	// so this constructor allows us to create the record from the item with less repetitive double-handling of field data
	[SetsRequiredMembers]
	public PropertyRecord(SavedItem fromItem) {
		this.Path = fromItem.Path;
		this.IsPriority = fromItem.IsPriority;
		this.DismissedReasons = fromItem.DismissedReasons;
		this.Notes = fromItem.Notes;
	}
	
	/// <summary>
	/// Add data about the property from the selected third-party API.
	/// </summary>
	/// <exception cref="HttpRequestException">Thrown when the third-party API returns a non-success status code.</exception>
	public async Task PopulateData() {
		this.Data = await this._api.GetPropertyByPath(this.Path);
	}

	public string GetFormattedAddress() {
		return this.Data?.Address?.ToString() ?? "";
	}
}