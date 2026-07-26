using Microsoft.AspNetCore.Components;
namespace PropertyPiles.Components.Layout;

public partial class SiteHeader : ComponentBase {
	
	private string? UserName { get; set; }
	
	protected override void OnInitialized() {
		base.OnInitialized();
		if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APP_USER_NAME"))) {
			this.UserName = Environment.GetEnvironmentVariable("APP_USER_NAME") + "ʼs";
		}
	}
	
}