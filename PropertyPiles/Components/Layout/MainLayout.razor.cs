using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using PropertyPiles.Containers;

namespace PropertyPiles.Components.Layout;

public partial class MainLayout : LayoutComponentBase, IAsyncDisposable {
	[Inject] 
	private AppState AppState { get; set; } = default!;
	
	[Inject] 
	private IJSRuntime JsRuntime { get; set; } = default!;
	private IJSObjectReference? _jsModule;
	
	[Inject]
	ProtectedLocalStorage LocalStorage { get; set; } = default!;
	
	private Action? _themeChangedHandler;

	private bool _isAuthorised;
	private bool _loginFailed;
	private string _inputPassword = "";
	private string _sessionKey = "AuthorisedPageSession";
	
	protected override async Task OnAfterRenderAsync(bool firstRender) {
		await this.CheckForCachedAuth();
		
		if (!firstRender) {
			await this.ApplyTheme();
			return;
		}

		_jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/Components/Layout/MainLayout.razor.js");
		
		_themeChangedHandler = () => InvokeAsync(async () => {
			await ApplyTheme();
			StateHasChanged();
		});
		AppState.OnChange += _themeChangedHandler;
        
		await this.ApplyTheme();
	}

	private async Task CheckForCachedAuth() {
		var authResult = await LocalStorage.GetAsync<bool>(this._sessionKey);
		this._isAuthorised = authResult.Success && authResult.Value;
		this.StateHasChanged();
	}
	
	private async Task Login() {
		string ? masterPassword = Environment.GetEnvironmentVariable("FRONT_END_PASSWORD");
		if (string.IsNullOrEmpty(masterPassword)) {
			this._loginFailed = true;
			this._isAuthorised = false;
			// TODO: Show a useful error message
		}
	
		if (this._inputPassword == masterPassword) {
			this._isAuthorised = true;
			this._loginFailed = false;
			await LocalStorage.SetAsync(this._sessionKey, true);
			StateHasChanged();
		}
		else {
			this._loginFailed = true;
			this._isAuthorised = false;
		}
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