using System.Linq;
using LastIRead.Data.Instance;
using Tests.fixtures;
using Xunit;
using Xunit.Priority;

namespace Tests {
	[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
	public class DataStoreTests : IClassFixture<DataStoreFixture> {
		public DataStoreTests(DataStoreFixture fixture) {
			_fixture = fixture;
		}

		private readonly DataStoreFixture _fixture;

		[Fact]
		[Priority(3)]
		public void DeleteTest() {
			var item = _fixture.DataStore.GetAll().First();

			_fixture.DataStore.Delete(item);

			Assert.Empty(_fixture.DataStore.GetAll());
		}

		[Fact]
		[Priority(1)]
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

		[Fact]
		[Priority(-1)]
		public void SaveTest() {
			_fixture.DataStore.Insert(_fixture.Readable);

			Assert.Single(_fixture.DataStore.GetAll());
		}

		[Fact]
		[Priority(2)]
		public void UpdateTest() {
			var item = _fixture.DataStore.GetAll().First();
			item.LogProgress(50.0);

			_fixture.DataStore.Update(item);

			var item2 = _fixture.DataStore.GetAll().First();

			Assert.Equal(item.Progress, item2.Progress);
		}
	}
}