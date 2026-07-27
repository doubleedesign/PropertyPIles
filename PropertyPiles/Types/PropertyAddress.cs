using System.Text.Json.Serialization;
namespace PropertyPiles.Types;

public record PropertyAddress {
	[JsonPropertyName("unitNumber")]
	public string UnitNumber { get; init; } = "";
	
	[JsonPropertyName("streetNumber")]
	public string Number { get; init; } = "";
    
	[JsonPropertyName("street")]
	public string Street { get; init; } = "";
    
	[JsonPropertyName("suburb")]
	public string Suburb { get; init; } = "";
    
	[JsonPropertyName("postcode")]
	public string Postcode { get; init; } = "";
	
	public override string ToString() {
		if (!string.IsNullOrEmpty(UnitNumber)) {
			return $"{UnitNumber}/{Number} {this.Street} {this.Suburb}";
		}
		
		return $"{this.Number} {this.Street}, {this.Suburb}";
	}
}