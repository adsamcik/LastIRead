using LiteDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LastIRead.data.database {
	class DataStore : IDisposable {
		private readonly LiteDatabase database;
		private readonly ILiteCollection<IReadable> collection;

		public DataStore() {
			database = AppDatabase.CreateDatabase();
			collection = database.GetReadablesCollection();
		}

		public void Update(IReadable readable) {
			collection.Update(readable);
		}

		public void Insert(IReadable readable) {
			collection.Insert(readable);
		}

		public void Insert(IEnumerable<IReadable> readables) {
			collection.Insert(readables);
		}

		public void Delete(IEnumerable<IReadable> readables) {
			foreach (var item in readables) {
				Delete(item);
			}
		}

		public void Delete(IReadable readable) {
			collection.Delete(readable.Id);
		}

		public IEnumerable<IReadable> GetSelected(string filter) {
			string strippedFilter = StripString(filter);
			var result = collection.FindAll();

			if (!string.IsNullOrEmpty(filter)) {
				result = result.Where((IReadable readable) => {
					var titleStripped = StripString(readable.Title);
					return titleStripped.Contains(strippedFilter, StringComparison.OrdinalIgnoreCase);
				});
			}

			return result.OrderBy(x => x.Title).ToArray();
		}

		public IEnumerable<IReadable> GetAll() {
			return collection.FindAll().ToArray();
		}

		public void Dispose() {
			database.Dispose();
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
