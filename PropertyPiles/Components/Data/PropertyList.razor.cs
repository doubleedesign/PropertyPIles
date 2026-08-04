using Microsoft.AspNetCore.Components;
using PropertyPiles.Services;
using PropertyPiles.Types;
using PropertyPiles.Utils;

namespace PropertyPiles.Components.Data;

public partial class PropertyList : ComponentBase {
	private List<PropertyRecord> Items = new();
	private bool _isLoading = true;
	
	[Inject]
	private ShortlistService ShortlistService { get; set; } = default!;
	
	[Parameter]
	public required string Id { get; set; }
	
	[Parameter]
	public required string ListName { get; set; }
	
	[Parameter]
	public required string Title { get; set; }
	
	
	protected override async Task OnInitializedAsync() {
		await base.OnInitializedAsync();
		this.Items = await this.ShortlistService.GetList(this.ListName);
		this.SortItemsByDaysOnMarket();
		
		this._isLoading = false;
	}
	
	private void SortItemsByDaysOnMarket() {
		this.Items = this.Items.OrderBy(p => p.Data?.DaysOnMarket).ToList();
	}
}