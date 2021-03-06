﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LastIRead.Import {
	/// <summary>
	///     Interface for data exporting.
	/// </summary>
	public interface IDataExporter {
		/// <summary>
		///     Array of all supported extensions
		/// </summary>
		public IEnumerable<string> ExportExtensions { get; }

		/// <summary>
		///     Exports data from readables to a file.
		/// </summary>
		/// <param name="bookmarks"></param>
		/// <param name="file"></param>
		Task Export(IEnumerable<IPersistentBookmark> bookmarks, FileInfo file);
	}
}