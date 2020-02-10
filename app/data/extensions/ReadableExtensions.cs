using System;

namespace LastIRead.data.extensions {
	public static class ReadableExtensions {
		public static string GetTitle(this IReadable readable) {
			return readable.LocalizedTitle ??
			       readable.OriginalTitle ??
			       string.Empty;
		}
	}
}