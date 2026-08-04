using Microsoft.AspNetCore.Components;
using PropertyPiles.Services;

namespace PropertyPiles.Components.Data;

public partial class PropertyListProvider : ComponentBase {
	[Inject]
	private ShortlistService ShortlistService { get; set; } = default!;
	
	[Parameter]
	public required RenderFragment ChildContent { get; set; }

	protected override async Task OnInitializedAsync() {
		await base.OnInitializedAsync();
		await this.ShortlistService.Init();
		
		if (this.ShortlistService.GetErrors().Count > 0) {
			// TODO: Show a single error message somewhere
		}
	}
}