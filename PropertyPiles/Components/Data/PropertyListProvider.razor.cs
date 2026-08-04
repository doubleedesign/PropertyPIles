using Microsoft.AspNetCore.Components;
using PropertyPiles.Services;
using PropertyPiles.Utils;

namespace PropertyPiles.Components.Data;

public partial class PropertyListProvider : ComponentBase {
	[Inject]
	private ShortlistService ShortlistService { get; set; } = default!;
	
	[Parameter]
	public required RenderFragment ChildContent { get; set; }

	protected override async Task OnInitializedAsync() {
		Logger.Info("Initializing property list provider");
		await base.OnInitializedAsync();
		await this.ShortlistService.Init();
		
		if (this.ShortlistService.GetErrors().Count > 0) {
			// TODO: Show a single error message somewhere
		}
	}
}