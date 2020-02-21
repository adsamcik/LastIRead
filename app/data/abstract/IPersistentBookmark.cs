using LastIRead.data.database;
using LiteDB;

namespace LastIRead {
	public interface IPersistentBookmark : IBookmark, IDatabaseItem {
		public new ObjectId? Id { get; set; }
	}
}