namespace PropertyPiles.Utils;

public static class Logger {
	private static int LABEL_WIDTH = 30;
	
	public static void Success(string message, string extra = "") {
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine("✅  " + message.PadRight(LABEL_WIDTH) + extra);
		Console.ResetColor();
	}
	
	public static void Error(string message, string extra = "") {
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine("❌  " + message.PadRight(LABEL_WIDTH) + extra);
		Console.ResetColor();
	}
	
	public static void Warning(string message, string extra = "") {
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine("⚠️ " + message.PadRight(LABEL_WIDTH) + extra);
		Console.ResetColor();
	}

	public static void Info(string message, string extra = "") {
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.WriteLine("📄 " + message.PadRight(LABEL_WIDTH) + extra);
		Console.ResetColor();
	}
}