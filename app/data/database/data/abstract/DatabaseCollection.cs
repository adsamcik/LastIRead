using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace LastIRead.data.database {
	/// <summary>
	///     Generic implementation of database collection providing basic methods to work with collection.
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

		public void Update(T item) {
			Collection.Update(item);
		}

		public void Insert(T item) {
			Collection.Insert(item);
		}

		public void Insert(IEnumerable<T> itemEnumerable) {
			Collection.Insert(itemEnumerable);
		}

		public void Delete(IEnumerable<T> itemEnumerable) {
			foreach (var item in itemEnumerable) {
				Delete(item);
			}
		}

		public void Delete(T item) {
			Collection.Delete(item.Id);
		}

		public IEnumerable<T> GetAll() {
			return Collection.FindAll().ToArray();
		}

		public void Upsert(T item) {
			Collection.Upsert(item);
		}
	}
}