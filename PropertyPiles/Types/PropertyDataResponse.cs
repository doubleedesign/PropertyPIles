using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PropertyPiles.Types;

internal class PropertyDataResponse {
	[JsonPropertyName("id")]
	public int? Id { get; set; }
	
	[JsonPropertyName("address")]
	public PropertyAddress? Address { get; set; }
	
	[JsonPropertyName("lifecycleStatus")]
	[AllowedValues("Live", "Under Offer", "Sold")]
	public string? Status { get; set; }
	
	[JsonPropertyName("daysOnMarket")]
	public int? DaysOnMarket { get; set; }
	
	[JsonPropertyName("isAuction")]
	public bool? IsAuction { get; set; }
	
	[JsonPropertyName("headline")]
	public string? ShortDescription { get; set; }
	
	[JsonPropertyName("price")]
	public string? Price { get; set; }
	
	[JsonPropertyName("landArea")]
	public string?LandArea { get; set; }
	
	[JsonPropertyName("bedrooms")]
	public int? Bedrooms { get; set; }
	
	[JsonPropertyName("bathrooms")]
	public int? Bathrooms { get; set; }
	
	[JsonPropertyName("carspaces")]
	public int? Carspaces { get; set; }
}