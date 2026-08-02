using Microsoft.AspNetCore.Components;
using PropertyPiles.Types;

namespace PropertyPiles.Components.Data;

public partial class PropertyInternet : ComponentBase {
	[Parameter]
	public NbnCoverageResponse? CoverageData { get; set; }
}