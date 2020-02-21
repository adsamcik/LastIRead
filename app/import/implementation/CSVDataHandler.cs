using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using LastIRead.Data.Instance;
using static System.Linq.AsyncEnumerable;

namespace LastIRead.Import {
	/// <summary>
	///     CSV Data handler providing CSV import and export for IReadables.
	/// </summary>
	public class CsvDataHandler : IDataImporter, IDataExporter {
		public IEnumerable<string> ExportExtensions => ImportExtensions;

		public async Task Export(IEnumerable<IPersistentBookmark> bookmarks, FileInfo file) {
			await using var writer = new StreamWriter(file.FullName);
			await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
			await csv.WriteRecordsAsync(bookmarks).ConfigureAwait(false);
		}

		public IEnumerable<string> ImportExtensions => new[] {"csv"};

		public async Task<IEnumerable<IPersistentBookmark>> Import(FileInfo file) {
			using var reader = new StreamReader(file.FullName);
			using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
			var records = csv.GetRecordsAsync<GenericBookmark>();

			return await records.Cast<IPersistentBookmark>().ToListAsync();
		}
	}
}