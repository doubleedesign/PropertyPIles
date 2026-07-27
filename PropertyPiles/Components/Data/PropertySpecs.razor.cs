using Microsoft.AspNetCore.Components;

namespace PropertyPiles.Components.Data;

public partial class PropertySpecs : ComponentBase {
	[Parameter]
	public int? Bedrooms { get; set; }
	
	[Parameter]
	public int? Bathrooms { get; set; }
	
	[Parameter]
	public int? Carspaces { get; set; }
}