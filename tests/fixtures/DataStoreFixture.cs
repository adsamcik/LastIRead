﻿using System;
using LastIRead.data.database;
using LastIRead.Data.Instance;

namespace Tests.fixtures {
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
}