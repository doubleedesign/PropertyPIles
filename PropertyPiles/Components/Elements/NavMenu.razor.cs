using Microsoft.AspNetCore.Components;
namespace PropertyPiles.Components.Elements;
	
public partial class NavMenu : ComponentBase {
	
	[Inject]
	private NavigationManager Nav { get; set; } = default!;
	

	private void Navigate(string anchor) { 
		// Blazor doesn't natively handle #anchor links, so this triggers manually what a normal on navigate event would do
		this.Nav.NavigateTo(anchor);
	}
}