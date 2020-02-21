using System;
using LastIRead;
using LastIRead.data.database;
using LastIRead.Data.Instance;

namespace Tests.fixtures {
	// ReSharper disable once ClassNeverInstantiated.Global
	public class DataStoreFixture : IDisposable {
		public readonly DataStore DataStore = new DataStore(AppDatabase.CreateDatabase());

		public readonly GenericBookmark Bookmark = new GenericBookmark {
			LocalizedTitle = "Localized",
			OriginalTitle = "Original",
			WebAddress = "Web",
			MaxProgress = 100,
			ProgressIncrement = 0.1
		};

		public DataStoreFixture() {
			Bookmark.LogProgress(50);
			DataStore.Delete(DataStore.GetAll());
		}

		public void Dispose() {
			DataStore.Dispose();
		}
	}
}