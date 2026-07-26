using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using PropertyPiles.Containers;

namespace PropertyPiles.Components.Layout;

public partial class MainLayout : LayoutComponentBase, IAsyncDisposable {
	[Inject] 
	private AppState AppState { get; set; } = default!;
	
	[Inject] 
	private IJSRuntime JsRuntime { get; set; } = default!;
	private IJSObjectReference? _jsModule;
	
	private Action? _themeChangedHandler;

	protected override async Task OnAfterRenderAsync(bool firstRender) {
		if (!firstRender) {
			await ApplyTheme();
			return;
		}

		_jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/Components/Layout/MainLayout.razor.js");
        
		_themeChangedHandler = () => InvokeAsync(async () => {
			await ApplyTheme();
			StateHasChanged();
		});
		AppState.OnChange += _themeChangedHandler;
        
		await ApplyTheme();
	}

	private async Task ApplyTheme() {
		if (_jsModule is null) return;
		await _jsModule.InvokeVoidAsync("applyTheme", AppState.GetTheme());
	}

	public async ValueTask DisposeAsync() {
		AppState.OnChange -= _themeChangedHandler;
		if (_jsModule is not null) {
			try { await _jsModule.DisposeAsync(); } catch (JSDisconnectedException) { }
		}
	}
}