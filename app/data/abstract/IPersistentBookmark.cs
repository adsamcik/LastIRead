using System.Collections.Generic;
using LastIRead.data.database;
using LiteDB;

namespace LastIRead {
	public interface IPersistentBookmark : IBookmark, IDatabaseItem { }
}