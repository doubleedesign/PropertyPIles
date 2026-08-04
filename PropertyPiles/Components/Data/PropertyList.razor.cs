using Microsoft.AspNetCore.Components;
using PropertyPiles.Services;
using PropertyPiles.Types;
using PropertyPiles.Utils;

namespace PropertyPiles.Components.Data;

public partial class PropertyList : ComponentBase, IDisposable {
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
		
		// Subscribe to the service events
		ShortlistService.OnSourceListLoaded += this.HandleSourceListLoaded;
		ShortlistService.OnDataHydrated += this.HandleHydratedListLoaded;
		
		Logger.Info($"PropertyList component initialized: {this.ListName.PadRight(12)}  {this.Items.Count} items loaded \t Loading state is {this._isLoading}");
	}

	private async void HandleSourceListLoaded(Dictionary<string, List<SavedItem>> lists) {
		this.Items = lists[this.ListName].Select(item => new PropertyRecord(item)).ToList();
		this._isLoading = false;
		
		Logger.Info($"HandleSourceListLoaded called for: {this.ListName.PadRight(12)}   {this.Items.Count} items loaded \t Loading state is {this._isLoading}"); 
		
		await InvokeAsync(StateHasChanged); 
	} 

	private async void HandleHydratedListLoaded(Dictionary<string, List<PropertyRecord>> lists) {
		// this.Items = lists[this.ListName];
		// this.SortItemsByDaysOnMarket();
		// this._isLoading = false;
		//
		// await InvokeAsync(StateHasChanged);
	}
	
	private void SortItemsByDaysOnMarket() {
		this.Items = this.Items.OrderBy(p => p.Data?.DaysOnMarket).ToList();
	}
	
	public void Dispose() {
		ShortlistService.OnSourceListLoaded -= this.HandleSourceListLoaded;
		ShortlistService.OnDataHydrated -= this.HandleHydratedListLoaded;
	}
}