using Microsoft.AspNetCore.Components;
using PropertyPiles.Extensions;
using PropertyPiles.Services;
using PropertyPiles.Types;

namespace PropertyPiles.Components.Data;

public partial class PropertyList : ComponentBase {
	private List<SavedItem> Items;
	
	[Parameter]
	public required string Id { get; set; }
	
	[Parameter]
	public required string ListName { get; set; }
	
	[Parameter]
	public required string Title { get; set; }
	
	
	protected override async Task OnInitializedAsync() {
		await base.OnInitializedAsync();
		this.Items = new ShortlistService().GetList(this.ListName);
		this.Items.LogToConsole();
	}
}