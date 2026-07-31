using System.Text.Json;
using System.Text.Json.Serialization;

namespace PropertyPiles.Utils;

/// <summary>
/// Utility to flatten a list of single-value dictionaries that are just a key-value pair of strings into a single dict.
/// e.g., TPG NBN availability response has data in a format like  "productFeatures": [ {"key1": "Value1" }, {"key2": "Value2" } ],
/// which is deserialized to <c>List{Dictionary{string, string}}</c> by default.
/// </summary>
public class JsonFlattenListOfDicts : JsonConverter<Dictionary<string, string>> {
	public override Dictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		var list = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(ref reader, options);
		return list?.SelectMany(d => d).ToDictionary(kv => kv.Key, kv => kv.Value) ?? new Dictionary<string, string>();
	}

	public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options) {
		JsonSerializer.Serialize(writer, value.Select(kv => new Dictionary<string, string> { { kv.Key, kv.Value } }), options);
	}
}