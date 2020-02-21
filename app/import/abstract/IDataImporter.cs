using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LastIRead.Import {
	/// <summary>
	///     Interface for data importing
	/// </summary>
	public interface IDataImporter {
		/// <summary>
		///     Array of import extensions.
		/// </summary>
		public IEnumerable<string> ImportExtensions { get; }

		/// <summary>
		///     Converts file from given format to new format.
		/// </summary>
		/// <param name="file">File</param>
		/// <returns>List of imported readables</returns>
		Task<IEnumerable<IBookmark>> Import(FileInfo file);
	}
}