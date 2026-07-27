using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
namespace PropertyPiles.Components.Elements;
	
public partial class NavMenu : ComponentBase {
	private string _activeAnchor = "";
	
	[Inject]
	private NavigationManager Nav { get; set; } = default!;
	
	[Inject]
	private IJSRuntime JsRuntime { get; set; } = default!;
	private IJSObjectReference? _jsModule;
	private DotNetObjectReference<NavMenu> _dotNetRef;

	protected override void OnInitialized() {
		// Checking for an active hash in the JS misses the first page load,
		// but because it's the URL on page load not after navigation, we can check it here
		var url = new Uri(Nav.Uri);
		var hash = url.Fragment;
		if (!string.IsNullOrEmpty(hash)) {
			this._activeAnchor = hash.Substring(1); // Remove the '#'
		}
		else {
			// Set what I know as the first link as the default for when the page loads without a hash
			// Ideally this would be dynamic but I'm all out of ideas for how to dynamically do it cleanly
			this._activeAnchor = "shortlist";
		}
	}


	protected override async Task OnAfterRenderAsync(bool firstRender) {
		if (!firstRender) return;

		_jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/Components/Elements/NavMenu.razor.js");
		_dotNetRef = DotNetObjectReference.Create(this);
		
		await _jsModule.InvokeVoidAsync("registerEventListeners", _dotNetRef);
	}

	[JSInvokable]
	public async Task SetActiveAnchor(string activeSection) {
		this._activeAnchor = activeSection;
	}
	
	[JSInvokable]
	public async Task OnScroll(string activeSection) {
		this._activeAnchor = activeSection;
		this.StateHasChanged();
	}
	
	private void Navigate(string anchor) { 
		if(anchor.StartsWith("#")) {
			anchor = anchor.Substring(1);
		}
		this._activeAnchor = anchor;
		
		this.Nav.NavigateTo($"#{anchor}", replace: true);
		StateHasChanged();
	}

	private string? AriaCurrent(string anchor) {
		return anchor == this._activeAnchor ? "page" : null;
	}
	
	public async ValueTask DisposeAsync() {
		_dotNetRef.Dispose();
		
		if (_jsModule is not null) {
			try {
				await _jsModule.InvokeVoidAsync("removeEventListeners", _dotNetRef);
				await _jsModule.DisposeAsync();
			}
			catch (JSDisconnectedException) { }
			catch (TaskCanceledException) { }
		}
	}
}