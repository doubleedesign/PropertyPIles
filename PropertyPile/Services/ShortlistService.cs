namespace PropertyPile.Services;
using System.Text.Json;

public class ShortlistService {
	private ShortlistData _shortlists = new ShortlistData();

	public ShortlistService() {
		this.LoadFile();
	}

	private void LoadFile() {
		using (StreamReader r = new StreamReader("data.json")) {  
			string json = r.ReadToEnd();
			this._shortlists = JsonSerializer.Deserialize<ShortlistData>(json) ??  new ShortlistData();
		}
	}
	
	public List<int> GetList(string name) {
		if (!_shortlists.ContainsKey(name)) {
			// TODO Sold is not a shortlist, the data needs to be handled in a different way so that sold properties are identified after data fetch
			// without coupling this class to the data service class
			//throw new ArgumentException($"Shortlist '{name}' does not exist.");
			return new List<int>();
		}
		
		return _shortlists[name];
	}
}