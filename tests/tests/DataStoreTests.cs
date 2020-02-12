using System.Collections.Generic;
using System.Linq;
using LastIRead;
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
		public void DeleteMultiple() {
			const int count = 10;
			for (var i = 0; i < count; i++) {
				_fixture.DataStore.Insert(new GenericReadable());
			}

			var all = _fixture.DataStore.GetAll().ToList();

			_fixture.DataStore.Delete(all);

			Assert.Equal(count, all.Count);
			Assert.Empty(_fixture.DataStore.GetAll());
		}

		[Fact]
		[Priority(3)]
		public void DeleteTest() {
			var item = _fixture.DataStore.GetAll().First();

			_fixture.DataStore.Delete(item);

			Assert.Empty(_fixture.DataStore.GetAll());
		}

		[Fact]
		public void GetSelected() {
			var dataStore = _fixture.DataStore;

			var validList = new List<IReadable> {
				new GenericReadable {
					OriginalTitle = "Great original title"
				},
				new GenericReadable {
					LocalizedTitle = "Great localized title"
				}
			};

			var invalidList = new List<IReadable> {
				new GenericReadable {
					OriginalTitle = "Great original story"
				},
				new GenericReadable {
					LocalizedTitle = "Great localized story"
				}
			};

			dataStore.Insert(validList);
			dataStore.Insert(invalidList);

			var selected = dataStore.GetSelected("  title  ").ToList();
			var all = dataStore.GetAll().ToList();

			dataStore.Delete(dataStore.GetAll());

			Assert.Equal(validList.Count + invalidList.Count, all.Count);
			Assert.Contains(
				selected,
				readable => validList.Any(
					item => item.LocalizedTitle == readable.LocalizedTitle ||
					        item.OriginalTitle == readable.OriginalTitle
				)
			);
			Assert.Empty(dataStore.GetAll());
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