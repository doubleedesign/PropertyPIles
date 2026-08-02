using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PropertyPiles.Types;

public class NbnCoverageResponse {
	[JsonPropertyName("type")]
	[AllowedValues("nbn", "opticomm")]
	public string? Type { get; set; }
	
	[JsonPropertyName("serviceClass")]
	public int ServiceClass  { get; set; }
	
	[JsonPropertyName("technology")]
	public string? Technology { get; set; }
	
	[JsonPropertyName("alternateTechnology")]
	public string? AlternateTechology { get; set; }
	
	
	[JsonPropertyName("speedPotential")]
	public SpeedPotentialData? SpeedPotential { get; set; }
}

public record SpeedPotentialData(
	[property: JsonPropertyName("downloadSpeed")] int? DownloadSpeed,
	[property: JsonPropertyName("uploadSpeed")]   int? UploadSpeed
);