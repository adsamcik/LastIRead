using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LastIRead.Import {
    /// <summary>
    /// Interface for data importing
    /// </summary>
    public interface IDataImporter {

        /// <summary>
        /// Array of import extensions.
        /// </summary>
        public string[] ImportExtensions { get; }

        /// <summary>
        /// Converts file from given format to new format.
        /// </summary>
        /// <exception cref="InvalidFormatException">Thrown when file format is not supported.</exception>
        /// <param name="file">File</param>
        /// <returns>List of imported readables</returns>
        Task<IList<IReadable>> Import(FileInfo file);
    }
}
