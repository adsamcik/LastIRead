using System;
using System.Linq;
using LastIRead.data.database;
using LastIRead.Data.Instance;
using Xunit;
using Xunit.Priority;

namespace Tests {
	// ReSharper disable once ClassNeverInstantiated.Global
	public class DataStoreFixture : IDisposable {
		public readonly DataStore DataStore = new DataStore();

		public readonly GenericReadable Readable = new GenericReadable {
			LocalizedTitle = "Localized",
			OriginalTitle = "Original",
			WebAddress = "Web",
			MaxProgress = 100,
			ProgressIncrement = 0.1
		};

		public DataStoreFixture() {
			Readable.LogProgress(50);
			DataStore.Delete(DataStore.GetAll());
		}

		public void Dispose() {
			DataStore.Dispose();
		}
	}

	[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
	public class DataStoreTests : IClassFixture<DataStoreFixture> {
		private readonly DataStoreFixture _fixture;

		public DataStoreTests(DataStoreFixture fixture) {
			_fixture = fixture;
		}

		[Fact, Priority(-1)]
		public void SaveTest() {
			_fixture.DataStore.Insert(_fixture.Readable);

			Assert.Single(_fixture.DataStore.GetAll());
		}

		[Fact, Priority(1)]
		public void ReadTest() {
			var item = (GenericReadable) _fixture.DataStore.GetAll().First();

			var readable = _fixture.Readable;
			Assert.Equal(readable.LocalizedTitle, item.LocalizedTitle);
			Assert.Equal(readable.OriginalTitle, item.OriginalTitle);
			Assert.Equal(readable.Progress, item.Progress);
			Assert.Equal(readable.ProgressIncrement, item.ProgressIncrement);
			Assert.Equal(readable.Abandoned, item.Abandoned);
			Assert.Equal(readable.Ongoing, item.Ongoing);
			Assert.Equal(readable.WebAddress, item.WebAddress);
			Assert.Equal(readable.History, item.History);
		}

		[Fact, Priority(2)]
		public void UpdateTest() {
			var item = _fixture.DataStore.GetAll().First();
			item.LogProgress(50.0);

			_fixture.DataStore.Update(item);

			var item2 = _fixture.DataStore.GetAll().First();

			Assert.Equal(item.Progress, item2.Progress);
		}
	}
}