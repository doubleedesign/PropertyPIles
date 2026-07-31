using System.Text.Json;
using System.Text.Json.Serialization;
using PropertyPiles.Extensions;

namespace PropertyPiles.Utils;

public class JsonStringToBool : JsonConverter<bool?> {
	public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		return reader.GetString()?.ToBool();
	}

	public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options) {
		if (value == null) writer.WriteNullValue();
		else writer.WriteStringValue(value.Value ? "Yes" : "No");
	}
}