using Humanizer;
using PropertyPiles.Types;

namespace PropertyPiles.Extensions;

public static class ListExtensions {
	
	public static void LogToConsole(this List<SavedItem> items) {
		Console.WriteLine(string.Concat(Enumerable.Repeat("==========", 15)));
		Console.WriteLine("Id".PadRight(16) + "Priority".PadRight(12) + "Address".PadRight(40) + "Notes".PadRight(30));
		Console.WriteLine(string.Concat(Enumerable.Repeat("----------", 15)));
		
		foreach (SavedItem item in items) {
			Console.Write($"{item.Id}".PadRight(16));
			Console.Write(item.IsPriority ? "Yes".PadRight(12) : "-".PadRight(12));
			Console.Write(item.GetShortAddress().PadRight(40));

			var combinedNotes = (item.Notes ?? []).ToList().Concat((item.DismissedReasons ?? []).ToList());
			Console.Write(string.Join(", ", combinedNotes).Transform(To.LowerCase).Transform(To.SentenceCase).PadRight(30));

			Console.WriteLine();
		}

		Console.WriteLine(string.Concat(Enumerable.Repeat("==========", 15)));
	}
}