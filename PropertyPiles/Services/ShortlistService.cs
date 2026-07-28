using System.Diagnostics;
using PropertyPiles.Types;

namespace PropertyPiles.Services;


public class ShortlistService {
	private FileService _fs = new();
	private Dictionary<string, List<SavedItem>> _rawlists = new();
	private Dictionary<string, List<PropertyRecord>> _shortlists = new();
	private Dictionary<string, List<String>> _fetchErrors = new();
	
	public ShortlistService() {
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
	}

	public async Task Init() {
		await this._fs.LoadFile();
		this.SortRawSavedItems();
	}

	/// <summary>
	/// Sort the raw data from the JSON file into the lists based on what we know just from there.
	/// </summary>
	/// <exception cref="NullReferenceException"></exception>
	private void SortRawSavedItems() {
		List<SavedItem>? rawList = this._fs.GetItemsFromFile();
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
		if (!this._shortlists.ContainsKey(listName)) {
			throw new ArgumentException($"List '{listName}' does not exist.");
		}

		if (listName == "sold") {
			// TODO: Skip fetching data for properties we already know are sold from previous fetches.
			// This will probably involve hoisting the shortlist service up so there's only one instance
			// - currently the PropertyList component instantiates it separately for each list.
			return;
		}

		if (!this._rawlists.ContainsKey(listName)) {
			throw new ArgumentException($"List '{listName}' does not exist.");
		}

		var rawList =  this._rawlists[listName];
		foreach (SavedItem item in rawList) {
			PropertyRecord property = new(item);
			
			try {
				await property.PopulateData();
				if (property.Data?.Status != "Sold") {
					this._shortlists[listName].Add(property);
				}
			}
			catch (HttpRequestException ex) {
				this._fetchErrors[listName].Add(ex.Message);
				// Included the SavedItem data if the API request failed
				this._shortlists[listName].Add(new PropertyRecord(item));
			}
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