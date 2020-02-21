using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LastIRead.Data.Instance;
using LastIRead.windows.main.pages;
using LiteDB;

namespace LastIRead.data.database {
	/// <summary>
	/// 	Generic implementation of database collection providing basic methods to work with collection.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class DatabaseCollection<T> : IAsyncDisposable, IDisposable where T : IDatabaseItem {
		protected readonly ILiteCollection<T> Collection;
		protected readonly LiteDatabase Database;

		protected DatabaseCollection(LiteDatabase database, ILiteCollection<T> collection) {
			Database = database;
			Collection = collection;
		}

		public async ValueTask DisposeAsync() {
			await Task.Run(Dispose);
		}

		public void Dispose() {
			Database.Dispose();
		}

		public void Update(T bookmark) {
			Collection.Update(bookmark);
		}

		public void Insert(T bookmark) {
			Collection.Insert(bookmark);
		}

		public void Insert(IEnumerable<T> readables) {
			Collection.Insert(readables);
		}

		public void Delete(IEnumerable<T> readables) {
			foreach (var item in readables) {
				Delete(item);
			}
		}

		public void Delete(T bookmark) {
			Collection.Delete(bookmark.Id);
		}

		public IEnumerable<T> GetAll() {
			return Collection.FindAll().ToArray();
		}
	}
}