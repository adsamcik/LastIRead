using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LastIRead.data.database {
	internal class DataStore : IDisposable {
		private readonly LiteDatabase _database;
		private readonly ILiteCollection<IReadable> _collection;

		public DataStore() {
			_database = AppDatabase.CreateDatabase();
			_collection = _database.GetReadablesCollection();
		}

		public void Update(IReadable readable) {
			_collection.Update(readable);
		}

		public void Insert(IReadable readable) {
			_collection.Insert(readable);
		}

		public void Insert(IEnumerable<IReadable> readables) {
			_collection.Insert(readables);
		}

		public void Delete(IEnumerable<IReadable> readables) {
			foreach (var item in readables) {
				Delete(item);
			}
		}

		public void Delete(IReadable readable) {
			_collection.Delete(readable.Id);
		}

		public IEnumerable<IReadable> GetSelected(string filter) {
			var strippedFilter = StripString(filter);
			var result = _collection.FindAll();

			if (!string.IsNullOrEmpty(filter)) {
				result = result.Where(readable => {
					var titleStripped = StripString(readable.Title);
					return titleStripped.Contains(strippedFilter, StringComparison.OrdinalIgnoreCase);
				});
			}

			return result.OrderBy(x => x.Title).ToArray();
		}

		public IEnumerable<IReadable> GetAll() {
			return _collection.FindAll().ToArray();
		}

		public void Dispose() {
			_database.Dispose();
		}


		private static string StripString(string text) {
			var selectedCharacters = text
				.Where(
					character =>
						!char.IsPunctuation(character) &&
						!char.IsWhiteSpace(character) &&
						!char.IsSeparator(character)
				);

			return string.Concat(selectedCharacters);
		}

	}
}
