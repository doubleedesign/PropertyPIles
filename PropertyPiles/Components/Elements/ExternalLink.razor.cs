using Microsoft.AspNetCore.Components;

namespace PropertyPiles.Components.Elements;

public partial class ExternalLink : ComponentBase {
	[Parameter]
	public required string Href { get; set; }
	
	[Parameter]
	public required string Label { get; set; }
}