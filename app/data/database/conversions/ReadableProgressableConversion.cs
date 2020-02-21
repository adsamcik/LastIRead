using LiteDB;

namespace LastIRead.data.database.conversions {
	public class ReadableProgressableConversion : IConversion {
		public void Convert(LiteDatabase database) {
			const string readableCollection = "readables";
			const string progressableCollection = "bookmarks";
			if (database.CollectionExists(readableCollection)) {
				database.RenameCollection(readableCollection, progressableCollection);
			}
		}
	}
}