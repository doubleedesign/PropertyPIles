using System.Text.Json.Serialization;
namespace PropertyPiles.Types;

public record PropertyAddress {
	[JsonPropertyName("streetNumber")]
	public string Number { get; init; } = "";
    
	[JsonPropertyName("street")]
	public string Street { get; init; } = "";
    
	[JsonPropertyName("suburb")]
	public string Suburb { get; init; } = "";
    
	[JsonPropertyName("postcode")]
	public string Postcode { get; init; } = "";
}