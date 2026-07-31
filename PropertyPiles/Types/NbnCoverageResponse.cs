using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using PropertyPiles.Extensions;
using PropertyPiles.Services.JsonParsers;
using PropertyPiles.Utils;

namespace PropertyPiles.Types;

public class NbnCoverageResponse {
	[JsonPropertyName("Services")]
	public Dictionary<string, int>? Services { get; set; }
	
	[JsonNestedProperty(["NBN", "ultraFastAvailable"])]
	public bool? UltraFastAvailable { get; set; }

	[JsonNestedProperty(["NBN", "superFastAvailable"])]
	public bool? SuperFastAvailable { get; set; }
	
	[JsonNestedProperty(["NBN", "productFeatures"])]
	public List<Dictionary<string, string>>? ProductFeatures { get; set; }
}





// public class NbnProductFeatures {
// 	// List of speed tier descriptions
// 	public List<string>? SpeedTiers;
// 	// Key-value pairs of available traffic classes and the fastest available speed in that class (in Mbps)
// 	public Dictionary<string, string>? TrafficClasses;
//
// 	public NbnProductFeatures(Dictionary<string, string>? raw) {
// 		if (raw == null) {
// 			Console.WriteLine("Raw product features are null, skipping parsing");
// 			return;
// 		}
// 		
// 		this.SpeedTiers = ParseList(raw, "Speed Tier Availability");
// 		this.TrafficClasses = this.ParseTrafficClassNames(raw).ToDictionary(
// 			tc => tc,
// 			tc => GetFastestAvailableForTrafficClass(tc, raw)
// 		);
//
// 		Logger.DebugObject(this.SpeedTiers);
// 	}
//
// 	private static List<string> ParseList(Dictionary<string, string> raw, string key, char separator = ',') {
// 		return raw.TryGetValue(key, out var val)
// 			? val.Split(separator).Select(s => s.Trim()).ToList()
// 			: new List<string>();
// 	}
//
// 	private List<string> ParseTrafficClassNames(Dictionary<string, string> raw) {
// 		Console.WriteLine("Parsing traffic class names from raw product features...");
// 		Logger.DebugObject(raw);
// 		HashSet<string> result = new();
// 		
// 		// Loop through the keys and find any containing TC1, TC2, or TC4 for which the value is Yes/true
// 		foreach (var (key, value) in raw) {
// 			Console.WriteLine($"Parsing {key}: {value}");
// 			if (key.Contains("TC1") && value.ToBool()) {
// 				result.Add("TC1");
// 			}
// 			if (key.Contains("TC2") && value.ToBool()) {
// 				result.Add("TC2");
// 			}
// 			if (key.Contains("TC4") && value.ToBool()) {
// 				result.Add("TC4");
// 			}
// 		}
//
// 		return result.ToList();
// 	}
//
// 	private string GetFastestAvailableForTrafficClass(string tc, Dictionary<string, string> raw) {
// 		foreach (var (key, value) in raw) {
// 			if(!key.StartsWith($"NFAS {tc}")) {
// 				continue;
// 			}
// 			
// 			if(!key.EndsWith("Capacity")) {
// 				continue;
// 			}
// 			
// 			// Extract the bit with Mbps
// 			var match = Regex.Match(key, @"(\d+)Mbps");
//
// 			if (match.Success) {
// 				return match.Groups[1].Value;
// 			}
// 		}
//
// 		return "Unknown";
// 	}
// }