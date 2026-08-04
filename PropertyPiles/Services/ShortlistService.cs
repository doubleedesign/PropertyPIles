using System.Collections.Concurrent;
using PropertyPiles.Types;
namespace PropertyPiles.Services;
using PropertyPiles.Utils;


public class ShortlistService {
	private FileService? _injectedFileService;
	private ListingDataService? _injectedListingDataService;
	private InternetCoverageService? _injectedInternetCoverageService;
	
	private Dictionary<string, List<PropertyRecord>> _shortlists = new();
	private List<string> _fetchErrors = new();

	private TaskCompletionSource<bool> _isInitialized = new();
	
	public ShortlistService(FileService injectedFileServiceRef, ListingDataService injectedDataServiceRef, InternetCoverageService injectedNbnServiceRef) {
		this._injectedFileService = injectedFileServiceRef;
		this._injectedListingDataService = injectedDataServiceRef;
		this._injectedInternetCoverageService = injectedNbnServiceRef;
	}

	/// <summary>
	/// Create or clear and re-create the lists, load/reload the shortlist data file, and populate the lists.
	/// This is its own method because it contains async operations (so can't be run in the constructor).
	/// </summary>
	public async Task Init() {
		if (this._injectedFileService == null) {
			throw new InvalidOperationException("ShortlistService has not been initialized with a FileService instance, so cannot load the data.");
		}
		
		try {
			this._isInitialized = new();
			this.ClearLists();
			this.CreateLists();
			await this.PopulateLists();
			this.OnDataLoaded();
		}
		catch (Exception ex) {
			Logger.Error(ex.Message);
		}
	}

	private void CreateLists() {
		this._shortlists.TryAdd("priority", new List<PropertyRecord>());
		this._shortlists.TryAdd("maybe", new List<PropertyRecord>());
		this._shortlists.TryAdd("dismissed", new List<PropertyRecord>());
		this._shortlists.TryAdd("sold", new List<PropertyRecord>());
	}

	private void ClearLists() {
		this._shortlists.Clear();
		this._fetchErrors.Clear();
	}

	private void OnDataLoaded() {
		this._isInitialized.SetResult(true);
	}

	private async Task PopulateLists() {
		if (this._injectedFileService == null) {
			throw new InvalidOperationException("ShortlistService has not been initialized with a FileService instance, so cannot populate data.");
		}
		if (this._injectedListingDataService == null) {
			throw new InvalidOperationException("ShortlistService has not been initialized with a ListingDataService instance, so cannot populate data.");
		}
		
		await this._injectedFileService.LoadFile();
		var rawList = this._injectedFileService.GetItemsFromFile();
		if (rawList == null) {
			throw new ArgumentException("ShortlistService could not load the shortlist data file, so cannot populate data.");
		}
		
		foreach (SavedItem item in rawList) {
			PropertyRecord property = new(item);
			try {
				await property.PopulateData(this._injectedListingDataService, this._injectedInternetCoverageService);
			}
			catch (HttpRequestException ex) {
				this._fetchErrors.Add(ex.Message);
			}
			finally {
				var listToAddTo = this.DetermineListForProperty(property);
				this._shortlists[listToAddTo].Add(property);
			}
		}
	}

	private string DetermineListForProperty(PropertyRecord property) {
		// Discard dismissed properties when they get sold
		if (property.Data?.Status == "Sold" && property.DismissedReasons?.Length < 1) {
			return "sold";
		}
		if (property.DismissedReasons?.Length > 0) {
			return "dismissed";
		}
		if (property.IsPriority) {
			return "priority";
		}

		return "maybe";
	}
	
	public async Task<List<PropertyRecord>> GetList(string name) {
		if (!this._shortlists.TryGetValue(name, out var list)) {
			throw new ArgumentException($"Shortlist '{name}' does not exist.");
		}

		// This method can be called from components while the data is still loading,
		// so awaiting the initialized state ensures it doesn't return until the data has all been populated
		await this._isInitialized.Task;
		
		return list;
	}
	
	public List<string> GetErrors() {
		// TODO: De-duplicate and format errors
		return this._fetchErrors;
	}
}