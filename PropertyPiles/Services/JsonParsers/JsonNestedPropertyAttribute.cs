using System.Text.Json;
using System.Text.Json.Serialization;
using PropertyPiles.Extensions;

namespace PropertyPiles.Services.JsonParsers;

public class JsonNestedPropertyAttribute(string[] hierarchy) : JsonConverterAttribute {
	public override JsonConverter CreateConverter(Type type) {
		Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

		if (underlyingType == typeof(bool)) {
			return new NestedBool(hierarchy);
		}

		if (underlyingType == typeof(string)) {
			return new NestedString(hierarchy);
		}

		if (underlyingType == typeof(int)) {
			return new NestedInt(hierarchy);
		}

		if (type == typeof(List<Dictionary<string, string>>)) {
			return new NestedList(hierarchy);
		}

		throw new NotImplementedException(
			$"Could not process nested attribute at path: {string.Join(".", hierarchy)} of type {type.FullName}"
		);
	}
	
	private class NestedBool(string[] hierarchy) : JsonConverter<bool> {
		public override bool Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) {
			var doc = JsonDocument.ParseValue(ref reader);
			var value = hierarchy.Aggregate(doc.RootElement, (el, key) => el.GetProperty(key));
			return value.GetString()?.ToBool() ?? false;
		}

		public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) {
			throw new NotImplementedException();
		}
	}

	private class NestedString(string[] hierarchy) : JsonConverter<string> {
		public override string Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) {
			var doc = JsonDocument.ParseValue(ref reader);
			var value  = hierarchy.Aggregate(doc.RootElement, (el, key) => el.GetProperty(key));
			return value.GetString() ?? "";
		}

		public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) {
			throw new NotImplementedException();
		}
	}
	
	private class NestedInt(string[] hierarchy) : JsonConverter<int> {
		public override int Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) {
			var doc = JsonDocument.ParseValue(ref reader);
			var value  = hierarchy.Aggregate(doc.RootElement, (el, key) => el.GetProperty(key));
			return value.GetInt32();
		}

		public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) {
			throw new NotImplementedException();
		}
	}
	
	private class NestedList(string[] hierarchy) : JsonConverter<List<Dictionary<string, string>>> {
		public override List<Dictionary<string, string>> Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) {
			var doc = JsonDocument.ParseValue(ref reader);
			var value = hierarchy.Aggregate(doc.RootElement, (el, key) => el.GetProperty(key));
			return value.EnumerateArray()
				.Select(el => el.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString() ?? ""))
				.ToList();
		}

		public override void Write(Utf8JsonWriter writer, List<Dictionary<string, string>> value, JsonSerializerOptions options) {
			throw new NotImplementedException();
		}
	}
}

