using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
namespace PropertyPiles.Components.Elements;

public partial class ThemeToggle : ComponentBase, IAsyncDisposable {
	private string _theme = "light";
	
	[Inject]
	private IJSRuntime JsRuntime { get; set; } = default!;
	private IJSObjectReference? _jsModule;
	
	protected override async Task OnAfterRenderAsync(bool firstRender) {
		if (firstRender) {
			_jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/Components/Elements/ThemeToggle.razor.js");
		}
		
		if (this._jsModule is null) return;
		
		this._theme = await _jsModule.InvokeAsync<string>("loadTheme");
	}

	private async Task OnToggle() { 
		var newTheme = this._theme == "light" ? "dark" : "light";
		await this.SetTheme(newTheme);
	}

	private async Task SetTheme(string theme) {
		if (this._jsModule is null) return;
		
		this._theme = theme;
		await _jsModule.InvokeVoidAsync("setTheme", theme);
	}

	private bool IsDarkMode() {
		return this._theme == "dark";
	}

	private string GetTheme() {
		return this._theme;
	}
	
	public async ValueTask DisposeAsync() {
		if (this._jsModule is null) return;

		await this._jsModule.DisposeAsync();
	}

}