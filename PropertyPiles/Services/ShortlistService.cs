using System.Collections.Concurrent;
using PropertyPiles.Types;
namespace PropertyPiles.Services;
using PropertyPiles.Utils;


public class ShortlistService {
	private readonly FileService? _injectedFileService;
	private readonly ListingDataService? _injectedListingDataService;
	private readonly InternetCoverageService? _injectedInternetCoverageService;
	
	private List<SavedItem>? _sourceList;
	private Dictionary<string, List<SavedItem>> _rawLists = new();
	private Dictionary<string, List<PropertyRecord>> _hydratedLists = new();
	private List<string> _fetchErrors = new();

	public event Action<Dictionary<string, List<SavedItem>>>? OnSourceListLoaded;
	public event Action<Dictionary<string, List<PropertyRecord>>>? OnDataHydrated;
	
	public ShortlistService(FileService injectedFileServiceRef, ListingDataService injectedDataServiceRef, InternetCoverageService injectedNbnServiceRef) {
		this._injectedFileService = injectedFileServiceRef;
		this._injectedListingDataService = injectedDataServiceRef;
		this._injectedInternetCoverageService = injectedNbnServiceRef;
	}

	/// <summary>
	/// Create or clear and re-create the lists, load/reload the shortlist data file, and populate the lists.
	/// This is its own method because it contains async operations (so can't be run in the constructor).
	/// This needs to be called by an appropriate high-level component that is only rendered once on the page (i.e., not in every PropertyList).
	/// </summary>
	public async Task Init() {
		if (this._injectedFileService == null) {
			throw new InvalidOperationException("ShortlistService has not been initialized with a FileService instance, so cannot load the data.");
		}
		
		Logger.Info("Initializing ShortlistService");
		
		this.ClearLists();
		this.CreateLists();
		
		try {
			await this._injectedFileService.LoadFile();
			this._sourceList = this._injectedFileService.GetItemsFromFile();
			this.PopulateLists();
			//await this.HydrateLists();
		}
		catch (Exception ex) {
			Logger.Error(ex.Message);
		}
	}
	
	private void ClearLists() {
		this._rawLists.Clear();
		this._hydratedLists.Clear();
		this._fetchErrors.Clear();
	}
	
	private void CreateLists() {
		this._rawLists.TryAdd("priority", new List<SavedItem>());
		this._rawLists.TryAdd("maybe", new List<SavedItem>());
		this._rawLists.TryAdd("dismissed", new List<SavedItem>());
		// Sold should never get populated here, but having an empty list simplifies handling the empty state while we wait for data
		this._rawLists.TryAdd("sold", new List<SavedItem>());
		
		this._hydratedLists.TryAdd("priority", new List<PropertyRecord>());
		this._hydratedLists.TryAdd("maybe", new List<PropertyRecord>());
		this._hydratedLists.TryAdd("dismissed", new List<PropertyRecord>());
		this._hydratedLists.TryAdd("sold", new List<PropertyRecord>());
	}

	/// <summary>
	/// Sort the user's list from their loaded data.json file, before any third-party data is added.
	/// Allows us to show an initial state on the front-end while we wait for hydration.
	/// </summary>
	private void PopulateLists() {
		if (this._sourceList == null) {
			throw new ArgumentException("ShortlistService could not load the shortlist data file, so cannot populate data.");
		}
		
		foreach (SavedItem item in this._sourceList) {
			var listToAddTo = this.DetermineListForSavedItem(item);
			if (listToAddTo != null) {
				this._rawLists[listToAddTo].Add(item);
			}
		}
		
		this.OnSourceListLoaded?.Invoke(this._rawLists);
	}

	/// <summary>
	/// Re-sort the lists while also adding and accounting for data from third-party APIs.
	/// </summary>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="ArgumentException"></exception>
	private async Task HydrateLists() {
		if (this._injectedFileService == null) {
			throw new InvalidOperationException("ShortlistService has not been initialized with a FileService instance, so cannot populate data.");
		}
		if (this._injectedListingDataService == null) {
			throw new InvalidOperationException("ShortlistService has not been initialized with a ListingDataService instance, so cannot populate data.");
		}
		if (this._sourceList == null) {
			throw new ArgumentException("ShortlistService could not load the shortlist data file, so cannot populate data.");
		}
		
		foreach (SavedItem item in this._sourceList) {
			PropertyRecord property = new(item);
			try {
				await property.PopulateData(this._injectedListingDataService, this._injectedInternetCoverageService);
			}
			catch (HttpRequestException ex) {
				this._fetchErrors.Add(ex.Message);
			}
			finally {
				var listToAddTo = this.DetermineListForProperty(property);
				if (listToAddTo != null) {
					this._hydratedLists[listToAddTo].Add(property);
				}
			}
		}
		
		this.OnDataHydrated?.Invoke(this._hydratedLists);
	}

	/// <summary>
	/// Determine which list a saved property item belongs in,
	/// based on what we know about it without accounting for data introduced by third-party APIs.
	/// </summary>
	/// <param name="property"></param>
	/// <returns></returns>
	private string? DetermineListForSavedItem(SavedItem property) {
		if (property.DismissedReasons != null && property.DismissedReasons.Any()) {
			return "dismissed";
		}
		if (property.IsPriority) {
			return "priority";
		}

		return "maybe";
	}

	/// <summary>
	/// Determine which list a fully populated PropertyRecord belongs in.
	/// Accounts for states that SavedItems are not aware of (e.g., property has been sold) on top of the same sorting logic as for SavedItems.
	/// </summary>
	/// <param name="property"></param>
	/// <returns></returns>
	private string? DetermineListForProperty(PropertyRecord property) {
		if (property.Data?.Status == "Sold") {
			// Discard dismissed properties when they get sold
			if (property.DismissedReasons != null && property.DismissedReasons.Any()) {
				return null;
			}
			
			return "sold";
		}

		return this.DetermineListForSavedItem(property);
	}
	
	public List<string> GetErrors() {
		// TODO: De-duplicate and format errors
		return this._fetchErrors;
	}
}