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
	public class CSVDataHandler : IDataImporter, IDataExporter {
		public string[] ExportExtensions => ImportExtensions;

		public async Task Export(IList<IReadable> readables, FileInfo file) {
			using var writer = new StreamWriter(file.FullName);
			using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
			await csv.WriteRecordsAsync(readables).ConfigureAwait(false);
		}

		public string[] ImportExtensions => new[] {"csv"};

		public async Task<IList<IReadable>> Import(FileInfo file) {
			using var reader = new StreamReader(file.FullName);
			using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
			var records = csv.GetRecordsAsync<GenericReadable>();

			return await records.Cast<IReadable>().ToListAsync();
		}
	}
}