using PropertyPiles.Types;

namespace PropertyPiles.Services;


public class ShortlistService {
	private FileService _fs = new FileService();
	private Dictionary<string, List<SavedItem>> _shortlists = new();
	
	public ShortlistService() {
		this._shortlists.Add("priority", new List<SavedItem>());
		this._shortlists.Add("maybe", new List<SavedItem>());
		this._shortlists.Add("dismissed", new List<SavedItem>());
		this._shortlists.Add("sold", new List<SavedItem>());
		
		this.SortLists();
	}

	private void SortLists() {
		List<SavedItem>? rawList = this._fs.GetItemsFromFile();
		if (rawList == null) {
			throw new NullReferenceException("Failed to load property lists from file.");
		}
		
		foreach (SavedItem item in rawList) {
			// TODO: If sold and NOT dismissed, put in sold list
			
			if(item.IsPriority)  {
				this._shortlists["priority"].Add(item);
				continue;
			}
			
			if(item.DismissedReasons != null && item.DismissedReasons.Length > 0) {
				this._shortlists["dismissed"].Add(item);
				continue;
			} 
			
			this._shortlists["maybe"].Add(item);
		}
	}
	
	public List<SavedItem> GetList(string name) {
		if (!_shortlists.ContainsKey(name)) {
			throw new ArgumentException($"Shortlist '{name}' does not exist.");
		}
		
		return _shortlists[name];
	}
}