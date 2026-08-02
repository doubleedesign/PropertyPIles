using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PropertyPiles.Types;

public class NbnCoverageResponse {
	[JsonPropertyName("type")]
	[AllowedValues("nbn", "opticomm")]
	public string? Type {
		get {
			if (field == "nbn") {
				return "NBN";
			}

			if (field == "opticomm") {
				return "Opticomm";
			}

			return field;
		}
		set;
	}
	
	[JsonPropertyName("serviceClass")]
	public int ServiceClass  { get; set; }
	
	[JsonPropertyName("technology")]
	public string? Technology { get; set; }
	
	[JsonPropertyName("alternateTechnology")]
	public string? AlternateTechology { get; set; }
	
	
	[JsonPropertyName("speedPotential")]
	public SpeedPotentialData? SpeedPotential { get; set; }

	public string GetFormattedDownloadSpeed() {
		if (SpeedPotential is not null) {
			return $"{SpeedPotential.DownloadSpeed} Mbps";
		}

		return "Unknown";
	}
	
	public string GetFormattedUploadSpeed() {
		if (SpeedPotential is not null) {
			return $"{SpeedPotential.UploadSpeed} Mbps";
		}

		return "";
	}
}

public record SpeedPotentialData(
	[property: JsonPropertyName("downloadSpeed")] int? DownloadSpeed,
	[property: JsonPropertyName("uploadSpeed")]   int? UploadSpeed
);