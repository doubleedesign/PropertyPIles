using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
namespace PropertyPiles.Components.Elements;
	
public partial class NavMenu : ComponentBase {
	[Inject]
	private NavigationManager Nav { get; set; } = default!;
	
	[Inject]
	private IJSRuntime JsRuntime { get; set; } = default!;
	private IJSObjectReference? _jsModule;
	private DotNetObjectReference<NavMenu> _dotNetRef;


	protected override async Task OnAfterRenderAsync(bool firstRender) {
		if (!firstRender) return;

		_jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/Components/Elements/NavMenu.razor.js");
		_dotNetRef = DotNetObjectReference.Create(this);
		
		await _jsModule.InvokeVoidAsync("registerEventListeners", _dotNetRef);

		// Run active menu link JS function once on load to set initial active state
		//await _jsModule.InvokeVoidAsync("activeMenuItemClass");
	}
	
	[JSInvokable]
	public async Task OnScroll(string activeSection) {
		if (_jsModule is null) return;
		Console.WriteLine($"OnScroll called, {activeSection}");
		this.Nav.NavigateTo($"#{activeSection}", replace: true);
		//await _jsModule.InvokeVoidAsync("activeMenuItemClass");
	}
	
	public async ValueTask DisposeAsync() {
		_dotNetRef.Dispose();
		
		if (_jsModule is not null) {
			try {
				await _jsModule.DisposeAsync();
			}
			catch (JSDisconnectedException) { }
			catch (TaskCanceledException) { }
		}
	}
	
	private void Navigate(string anchor) { 
		this.Nav.NavigateTo(anchor, replace: true);
	}
}