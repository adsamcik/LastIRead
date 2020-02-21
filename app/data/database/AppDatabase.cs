using LastIRead.data.database.conversions;
using LastIRead.tools;
using LiteDB;

namespace LastIRead {
	public static class AppDatabase {
		private const string BookmarkCollection = "bookmarks";
		private const string PreferenceCollection = "preferences";

		private const string Path = "reading_data.db";

		static AppDatabase() {
			using var database = CreateDatabase();

			foreach (var conversion in ReflectionTools.GetImplementors<IConversion>()) {
				conversion.Convert(database);
			}
		}

		public static LiteDatabase CreateDatabase() {
			return new LiteDatabase(Path);
		}

		/// <summary>
		///     Gets collection for progress items.
		/// </summary>
		/// <param name="database">Lite database instance</param>
		/// <returns>Progress collection</returns>
		public static ILiteCollection<IPersistentBookmark> GetBookmarkCollection(this LiteDatabase database) {
			return database.GetCollection<IPersistentBookmark>(BookmarkCollection);
		}

		public static ILiteCollection<IPreference> GetPreferenceCollection(this LiteDatabase database) {
			return database.GetCollection<IPreference>(PreferenceCollection);
		}
	}
}