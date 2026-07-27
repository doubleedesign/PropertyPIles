using Microsoft.AspNetCore.Components;
using PropertyPiles.Services;
using PropertyPiles.Types;

namespace PropertyPiles.Components.Data;

public partial class PropertyList : ComponentBase {
	private List<PropertyRecord> Items = new();
	private ShortlistService _service = new();
	private bool _isLoading = true;
	
	[Parameter]
	public required string Id { get; set; }
	
	[Parameter]
	public required string ListName { get; set; }
	
	[Parameter]
	public required string Title { get; set; }
	
	
	protected override async Task OnInitializedAsync() {
		await base.OnInitializedAsync();
		await this._service.Init();
		
		this.Items = await this._service.GetList(this.ListName);
		
		if (this._service.GetErrorsForList(this.ListName).Count > 0) {
			// TODO: Show a single error message somewhere
		}
		
		this._isLoading = false;
	}
}