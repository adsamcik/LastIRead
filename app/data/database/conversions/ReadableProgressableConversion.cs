using System.Linq;
using System.Reflection;
using LastIRead.Data.Instance;
using LiteDB;

namespace LastIRead.data.database.conversions {
	/// <summary>
	///     Converts database from readables to bookmarks.
	/// </summary>
	public class ReadableProgressableConversion : IConversion {
		public void Convert(LiteDatabase database) {
			const string readableCollection = "readables";
			const string progressableCollection = "bookmarks";
			if (!database.CollectionExists(readableCollection)) {
				return;
			}

			var collection = database.GetCollection(readableCollection);
			var castList = collection.FindAll().ToArray();

			foreach (var item in castList) {
				item["_type"] = $"{typeof(GenericBookmark)}, {Assembly.GetExecutingAssembly().GetName().Name}";
			}

			collection.Update(castList);
			database.RenameCollection(readableCollection, progressableCollection);
		}
	}
}