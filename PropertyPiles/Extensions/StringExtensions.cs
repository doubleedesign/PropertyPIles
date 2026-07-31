namespace PropertyPiles.Extensions;

public static class StringExtensions {

	public static bool ToBool(this string value) {
		return value.Trim().ToLower() switch {
			"yes" => true,
			_ => false
		};
	}
}