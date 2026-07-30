using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
namespace PropertyPiles.Types;

public record PropertyAddress {
	[JsonPropertyName("unitNumber")]
	public string UnitNumber { get; init; } = "";
	
	[JsonPropertyName("streetNumber")]
	public string StreetNumber { get; init; } = "";

	public string Number => !string.IsNullOrEmpty(this.UnitNumber) ? $"{this.UnitNumber}/{this.StreetNumber}" : this.StreetNumber;

	[JsonPropertyName("street")]
	public string Street { get; init; } = "";
	
	public string StreetType => this.Street.Split(" ").LastOrDefault() ?? "";
	public string StreetName => this.Street.Replace(this.StreetType, "");
    
	[JsonPropertyName("suburb")]
	public string Suburb { get; init; } = "";
	
	[JsonPropertyName("state")]
	public string State { get; init; } = "";
    
	[JsonPropertyName("postcode")]
	public string Postcode { get; init; } = "";
	
	public override string ToString() {
		if (!string.IsNullOrEmpty(UnitNumber)) {
			return $"{UnitNumber}/{Number} {this.Street} {this.Suburb}";
		}
		
		return $"{this.Number} {this.Street}, {this.Suburb}";
	}

	/// <summary>
	/// Get the address values in the format required for NBN availability queries.
	/// </summary>
	/// <returns></returns>
	public Dictionary<string, string> ToKeyValues() {
		return new Dictionary<string, string> {
			{ "streetNumber", this.Number },
			{ "streetName", this.StreetName },
			{ "streetType", this.StreetType },
			{ "suburb", this.Suburb },
			{ "state", this.State },
			{ "postcode", this.Postcode }
		};
	}
}