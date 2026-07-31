using System.Diagnostics.CodeAnalysis;
using PropertyPiles.Services;
using PropertyPiles.Utils;

namespace PropertyPiles.Types;

public class PropertyRecord : SavedItem {
	internal PropertyDataResponse? Data;
	internal NbnCoverageResponse? NbnCoverage;

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
	/// <exception cref="HttpRequestException">Thrown when a third-party API returns a non-success status code.</exception>
	public async Task PopulateData(ListingDataService listingDataService, NbnCoverageService? nbnCoverageService) {
		this.Data = await listingDataService.GetPropertyByPath(this.Path);
		if (!this.IncludeNbnCoverage() || nbnCoverageService == null) return;
		
		 try {
		 	this.NbnCoverage = await nbnCoverageService.GetCoverageForProperty(this);
		 }
		 catch (Exception ex) {
		 	Logger.Error(ex.ToString());
		 }

	}
	
	private bool IncludeNbnCoverage() {
		var disableForStatuses = new List<string?> { "Sold", "Archived" };
	
		if (this.DismissedReasons?.Length > 0) return false;
		if (this.Data?.Status == null) return false;
		if (disableForStatuses.Contains(this.Data?.Status)) return false;
	
		return true;
	}
	
	public string GetFormattedAddress() {
		return this.Data?.Address?.ToString() ?? "";
	}

	public Dictionary<string, string>? GetAddressData() {
		return this.Data?.Address?.ToKeyValues();
	}
}