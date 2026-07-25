using Microsoft.AspNetCore.Components;
using PropertyPiles.Services;

namespace PropertyPiles.Components.Data;

public partial class PropertyList : ComponentBase {
	[Parameter]
	public required string Id { get; set; }
	
	[Parameter]
	public required string ListName { get; set; }
	
	[Parameter]
	public required string Title { get; set; }
	
	
	protected override async Task OnInitializedAsync() {
		await base.OnInitializedAsync();
		
		List<int> ids = new ShortlistService().GetList(this.ListName);
		Console.WriteLine("Found " + ids.Count);
	}
}