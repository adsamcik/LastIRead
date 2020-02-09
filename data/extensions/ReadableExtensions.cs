namespace LastIRead.data.extensions {
	public static class ReadableExtensions {
		public static string GetTitle(this IReadable readable) => readable.LocalizedTitle ?? readable.OriginalTitle;
	}
}