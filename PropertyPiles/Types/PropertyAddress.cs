using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
namespace PropertyPiles.Types;

public record PropertyAddress {
	[JsonPropertyName("unitNumber")]
	public string UnitNumber { get; init; } = "";
	
	[JsonPropertyName("streetNumber")]
	public string StreetNumber { get; init; } = "";

	public string Number => !string.IsNullOrEmpty(this.UnitNumber) ? $"{this.UnitNumber.Trim()}/{this.StreetNumber.Trim()}" : this.StreetNumber.Trim();

	[JsonPropertyName("street")]
	public string Street { get; init; } = "";
	
	public string StreetType => this.Street.Split(" ").LastOrDefault()?.Trim() ?? "";
	public string StreetName => this.Street.Replace(this.StreetType, "").Trim();
    
	[JsonPropertyName("suburb")]
	public string Suburb { get; init; } = "";
	
	[JsonPropertyName("state")]
	public string State { get; init; } = "";
    
	[JsonPropertyName("postcode")]
	public string Postcode { get; init; } = "";

	public string ToString(bool withPostcode = false, bool withState = false, bool verboseUnitSyntax = false) {
		string result;

		if (!string.IsNullOrEmpty(UnitNumber)) {
			result = verboseUnitSyntax ? $"Unit {UnitNumber}, {StreetNumber}" : $"{UnitNumber}/{StreetNumber}";
		}
		else {
			result = $"{this.Number}";
		}

		result += $" {this.Street}, {this.Suburb}";
		
		if (withState) {
			result += $" {this.State}";
		}

		if (withPostcode) {
			result += $" {this.Postcode}";
		}

		return result;
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