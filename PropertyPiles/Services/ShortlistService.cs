using PropertyPiles.Types;
namespace PropertyPiles.Services;


public class ShortlistService {
	private FileService? _injectedFileService;
	private ListingDataService? _injectedListingDataService;
	private InternetCoverageService? _injectedInternetCoverageService;
	
	private Dictionary<string, List<SavedItem>> _rawlists = new();
	private Dictionary<string, List<PropertyRecord>> _shortlists = new();
	private Dictionary<string, List<String>> _fetchErrors = new();

	private bool _isInitialized = false;
	
	// One semaphore per list to allow different lists to populate concurrently (from multiple instances of PropertyList on the same page)
	// while preventing the same list from being populated multiple times simultaneously
	private readonly Dictionary<string, SemaphoreSlim> _locks = new();
	private readonly HashSet<string> _populated = new();
	
	public ShortlistService() {
	}

	/// <summary>
	/// Initialize the class with the injected providers and empty lists to sort the data into.
	/// Set _isInitialized when done, to ensure this only gets called once if multiple components try to initialize it.
	/// Otherwise, we get duplicate entries in the lists.
	/// </summary>
	/// <param name="injectedFileServiceRef">The singleton FileService injected into the Blazor component that calls this service.</param>
	/// <param name="injectedDataServiceRef">The singleton ListingDataService injected into the Blazor component that calls this service.</param>
	/// <param name="injectedNbnServiceRef">The singleton InternetCoverageService injected into the Blazor component that calls this service.</param>
	public async Task Init(FileService injectedFileServiceRef, ListingDataService injectedDataServiceRef, InternetCoverageService injectedNbnServiceRef) {
		if (this._isInitialized) return;
		
		this._locks.Add("priority", new SemaphoreSlim(1, 1));
		this._locks.Add("maybe", new SemaphoreSlim(1, 1));
		this._locks.Add("dismissed", new SemaphoreSlim(1, 1));
		this._locks.Add("sold", new SemaphoreSlim(1, 1));
		
		this._rawlists.Add("priority", new List<SavedItem>());
		this._rawlists.Add("maybe", new List<SavedItem>());
		this._rawlists.Add("dismissed", new List<SavedItem>());
		
		this._fetchErrors.Add("priority", new List<string>());
		this._fetchErrors.Add("maybe", new List<string>());
		this._fetchErrors.Add("dismissed", new List<string>());
		this._fetchErrors.Add("sold", new List<string>());
		
		this._shortlists.Add("priority", new List<PropertyRecord>());
		this._shortlists.Add("maybe", new List<PropertyRecord>());
		this._shortlists.Add("dismissed", new List<PropertyRecord>());
		this._shortlists.Add("sold", new List<PropertyRecord>());
		
		this._injectedFileService = injectedFileServiceRef;
		this._injectedListingDataService = injectedDataServiceRef;
		this._injectedInternetCoverageService = injectedNbnServiceRef;
		
		await this._injectedFileService.LoadFile();
		this.SortRawSavedItems();

		this._isInitialized = true;
	}

	/// <summary>
	/// Sort the raw data from the JSON file into the lists based on what we know just from there.
	/// </summary>
	/// <exception cref="NullReferenceException"></exception>
	private void SortRawSavedItems() {
		if(this._injectedFileService == null) {
			throw new InvalidOperationException("ShortlistService has not been initialized with a FileService instance. Call Init() before calling SortRawSavedItems().");
		}
		
		List<SavedItem>? rawList = this._injectedFileService.GetItemsFromFile();
		if (rawList == null) {
			throw new NullReferenceException("Failed to load property lists from file.");
		}
		
		foreach (SavedItem item in rawList) {
			if(item.IsPriority) {
				this._rawlists["priority"].Add(item);
				continue;
			}
			
			if(item.DismissedReasons != null && item.DismissedReasons.Length > 0) {
				this._rawlists["dismissed"].Add(item);
				continue;
			}

			this._rawlists["maybe"].Add(item);
		}
	}

	private async Task PopulateList(string listName) {
		if (this._injectedListingDataService == null) {
			throw new InvalidOperationException("ShortlistService has not been initialized with a ListingDataService instance. Call Init() before calling PopulateList().");
		}
		
		if (!this._shortlists.ContainsKey(listName)) {
			throw new ArgumentException($"List '{listName}' does not exist.");
		}
		
		// Sold list is populated as a side effect of other lists, so we don't need to populate it here
		if (listName == "sold") return;

		if (!this._rawlists.ContainsKey(listName)) {
			throw new ArgumentException($"List '{listName}' does not exist.");
		}
		
		var sem = _locks[listName];
		await sem.WaitAsync();
		try {
			// Already populated by a concurrent caller
			if (_populated.Contains(listName)) return;

			// Store snapshot to avoid "collection was modified" if anything else touches it
			var rawList = _rawlists[listName].ToList();
            
			foreach (SavedItem item in rawList) {
				PropertyRecord property = new(item);
				try {
					await property.PopulateData(this._injectedListingDataService, this._injectedInternetCoverageService);
					if (property.Data?.Status != "Sold") {
						this._shortlists[listName].Add(property);
					}
				}
				catch (HttpRequestException ex) {
					this._fetchErrors[listName].Add(ex.Message);
					// Include just the the SavedItem data if the API request failed
					this._shortlists[listName].Add(new PropertyRecord(item));
				}
			}
            
			_populated.Add(listName);
		}
		finally {
			sem.Release();
		}
	}
	
	public async Task<List<PropertyRecord>> GetList(string name) {
		await this.PopulateList(name);
		
		if (!this._shortlists.ContainsKey(name)) {
			throw new ArgumentException($"Shortlist '{name}' does not exist.");
		}
		
		return _shortlists[name];
	}
	
	public List<string> GetErrorsForList(string listName) {
		if(!this._fetchErrors.ContainsKey(listName)) {
			throw new ArgumentException($"Shortlist '{listName}' does not exist.");
		}
		
		return this._fetchErrors[listName];
	}
}