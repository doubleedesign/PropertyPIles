using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PropertyPiles.Services.JsonParsers;

namespace PropertyPiles.Types;

public class NbnCoverageResponse {
	[JsonPropertyName("type")]
	[AllowedValues("nbn", "opticomm")]
	public string? Type { get; set; }
	
	[JsonPropertyName("serviceClass")]
	public int ServiceClass  { get; set; }
	
	[JsonPropertyName("technology")]
	public string? Technology { get; set; }
	
	[JsonPropertyName("alternateTechology")]
	public string? AlternateTechology { get; set; }
	
	[JsonNestedProperty(["speedPotential", "downloadSpeed"])]
	public int? DownloadSpeed { get; set; }
		
	[JsonNestedProperty(["speedPotential", "uploadSpeed"])]
	public int? UploadSpeed { get; set; }
}