using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LastIRead.Data.Instance;
using LastIRead.windows.main.pages;
using LiteDB;

namespace LastIRead.data.database {
	public class DataStore : IAsyncDisposable, IDisposable {
		private readonly ILiteCollection<IBookmark> _collection;
		private readonly LiteDatabase _database;

		public DataStore() {
			_database = AppDatabase.CreateDatabase();
			_collection = _database.GetBookmarkCollection();
		}

		public async ValueTask DisposeAsync() {
			await Task.Run(Dispose);
		}

		public void Dispose() {
			_database.Dispose();
		}

		public void Update(IBookmark bookmark) {
			_collection.Update(bookmark);
		}

		public void Insert(IBookmark bookmark) {
			_collection.Insert(bookmark);
		}

		public void Insert(IEnumerable<IBookmark> readables) {
			_collection.Insert(readables);
		}

		public void Delete(IEnumerable<IBookmark> readables) {
			foreach (var item in readables) {
				Delete(item);
			}
		}

		public void Delete(IBookmark bookmark) {
			_collection.Delete(bookmark.Id);
		}

		public IEnumerable<IUserBookmark> GetSelected(string filter, FilterData filterData) {
			var strippedFilter = StripString(filter);
			var result = _collection.FindAll();

			if (!string.IsNullOrEmpty(filter)) {
				result = result.Where(
					readable => Contains(readable.LocalizedTitle, strippedFilter) ||
					            Contains(readable.OriginalTitle, strippedFilter)
				);
			}

			if (filterData.Hide.HasFlag(Filter.Reading)) {
				result = result.Where(
					readable => !readable.Abandoned && (readable.Ongoing || readable.Progress < readable.MaxProgress)
				);
			}

			if (filterData.Hide.HasFlag(Filter.Abandoned)) {
				result = result.Where(readable => !readable.Abandoned);
			}

			if (filterData.Hide.HasFlag(Filter.Ended)) {
				result = result.Where(readable => readable.Ongoing);
			}

			if (filterData.Hide.HasFlag(Filter.Finished)) {
				result = result.Where(readable => readable.Ongoing || readable.Progress < readable.MaxProgress);
			}

			if (filterData.Hide.HasFlag(Filter.Ongoing)) {
				result = result.Where(readable => !readable.Ongoing);
			}

			return result
			       .Select(x => new WrapperUserBookmark(x))
			       .OrderBy(x => x.Title)
			       .ToArray();
		}

		private static bool Contains(string? title, string filter) {
			if (title == null) return false;

			var strippedTitle = StripString(title);
			return strippedTitle.Contains(filter, StringComparison.InvariantCultureIgnoreCase);
		}

		public IEnumerable<IBookmark> GetAll() {
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