using Microsoft.AspNetCore.Components;
using PropertyPiles.Services;
using PropertyPiles.Types;

namespace PropertyPiles.Components.Data;

public partial class PropertyList : ComponentBase {
	private List<PropertyRecord> Items = new();
	private bool _isLoading = true;
	
	[Inject]
	private ListingDataService InjectedListingDataService { get; set; } = default!;
	
	[Inject]
	private FileService InjectedFileService { get; set; } = default!;
	
	[Inject]
	private ShortlistService InjectedShortlistService { get; set; } = default!;
	
	[Inject]
	private InternetCoverageService InjectedInternetCoverageService { get; set; } = default!;
	
	[Parameter]
	public required string Id { get; set; }
	
	[Parameter]
	public required string ListName { get; set; }
	
	[Parameter]
	public required string Title { get; set; }
	
	
	protected override async Task OnInitializedAsync() {
		await base.OnInitializedAsync();
		await this.InjectedShortlistService.Init(this.InjectedFileService, this.InjectedListingDataService, this.InjectedInternetCoverageService);
		this.Items = await this.InjectedShortlistService.GetList(this.ListName);
		
		if (this.InjectedShortlistService.GetErrorsForList(this.ListName).Count > 0) {
			// TODO: Show a single error message somewhere
		}
		
		this._isLoading = false;
	}
}