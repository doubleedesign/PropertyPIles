using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PropertyPiles.Containers;

namespace PropertyPiles.Components.Elements;

public partial class ThemeToggle : ComponentBase, IAsyncDisposable {
	[Inject]
	private AppState AppState { get; set; } = default!;
	
	[Inject]
	private IJSRuntime JsRuntime { get; set; } = default!;
	private IJSObjectReference? _jsModule;
	
	
	protected override async Task OnAfterRenderAsync(bool firstRender) {
		if (!firstRender) return;
		
		_jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/Components/Elements/ThemeToggle.razor.js");
		
		var theme = await _jsModule.InvokeAsync<string>("loadTheme");
		AppState.SetTheme(theme);
	}

	private async Task OnToggle() { 
		var newTheme = AppState.GetTheme() == "light" ? "dark" : "light";
		await this.SetTheme(newTheme);
	}

	private async Task SetTheme(string theme) {
		if (this._jsModule is null) return;
		
		await _jsModule.InvokeVoidAsync("setTheme", theme);
		AppState.SetTheme(theme);
	}

	public async ValueTask DisposeAsync() {
		if (_jsModule is not null) {
			try {
				await _jsModule.DisposeAsync();
			}
			catch (JSDisconnectedException) { }
			catch (TaskCanceledException) { }
		}
	}
}