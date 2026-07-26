namespace PropertyPiles.Containers;

public class AppState {
	private string _theme = "light";

	public event Action? OnChange;
    
	public void SetTheme(string theme) {
		if (this._theme == theme) return;
		
		this._theme = theme;
		this.OnChange?.Invoke();
	}

	public string GetTheme() {
		return this._theme;
	}
	
	public bool IsDarkMode() {
		return this._theme == "dark";
	}
}