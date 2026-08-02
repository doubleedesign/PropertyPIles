using System.Text.Json;
using Spectre.Console;

namespace PropertyPiles.Utils;

public static class Logger {
	private static int LABEL_WIDTH = 30;

	static Logger() {
		AnsiConsole.Profile.Capabilities.Ansi = true;
		AnsiConsole.Profile.Capabilities.ColorSystem = ColorSystem.TrueColor;
	}

	public static void Success(string message, string extra = "") {
		var label = ("✅  " + message).PadRight(LABEL_WIDTH);
		AnsiConsole.MarkupLine($"[green]{label}{extra}[/]");
	}

	public static void Error(string message, string extra = "") {
		var label = ("❌  " + message).PadRight(LABEL_WIDTH);
		AnsiConsole.MarkupLine($"[red]{label}{extra}[/]");
	}

	public static void Warning(string message, string extra = "") {
		var label = ("⚠️  " + message).PadRight(LABEL_WIDTH);
		AnsiConsole.MarkupLine($"[yellow]{label}{extra}[/]");
	}

	public static void Info(string message, string extra = "") {
		var label = ("📄 " + message).PadRight(LABEL_WIDTH);
		AnsiConsole.MarkupLine($"[blue]{label}{extra}[/]");
	}

	public static void DebugObject(object? theObject) {
		if (theObject is null) {
			Warning("Object passed to Logger.DebugObject() is null");
			return;
		}

		var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
		string jsonString = JsonSerializer.Serialize(theObject, options);
		AnsiConsole.MarkupLine($"[grey]{jsonString.EscapeMarkup()}[/]");
	}
}