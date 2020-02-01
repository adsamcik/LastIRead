using LiteDB;

namespace LastIRead {
	public static class AppDatabase {
		private const string ReadableCollection = "readables";
		private const string Path = "reading_data.db";

		public static LiteDatabase CreateDatabase() {
			return new LiteDatabase(Path);
		}

		public static ILiteCollection<IReadable> GetReadablesCollection(this LiteDatabase database) {
			return database.GetCollection<IReadable>(ReadableCollection);
		}
	}
}