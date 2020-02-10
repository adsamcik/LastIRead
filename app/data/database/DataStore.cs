using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LastIRead.data.extensions;
using LiteDB;

namespace LastIRead.data.database {
	internal class DataStore : IAsyncDisposable, IDisposable {
		private readonly ILiteCollection<IReadable> _collection;
		private readonly LiteDatabase _database;

		public DataStore() {
			_database = AppDatabase.CreateDatabase();
			_collection = _database.GetReadablesCollection();
		}

		public async ValueTask DisposeAsync() {
			await Task.Run(Dispose);
		}

		public void Dispose() {
			_database.Dispose();
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
				result = result.Where(
					readable => Contains(readable.LocalizedTitle, strippedFilter) ||
					            Contains(readable.OriginalTitle, strippedFilter)
				);
			}

			return result.OrderBy(x => x.GetTitle()).ToArray();
		}

		private static bool Contains(string? title, string filter) {
			if (title == null) return false;

			var strippedTitle = StripString(title);
			return strippedTitle.Contains(filter, StringComparison.InvariantCultureIgnoreCase);
		}

		public IEnumerable<IReadable> GetAll() {
			return _collection.FindAll().ToArray();
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