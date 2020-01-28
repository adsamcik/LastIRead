using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LastIRead.Import {
    public interface IDataImporter {
        /// <summary>
        /// Converts file from given format to new format.
        /// </summary>
        /// <exception cref="InvalidFormatException">Thrown when file format is not supported.</exception>
        /// <param name="file">File</param>
        /// <returns>List of imported readables</returns>
        List<IReadable> Import(FileInfo file);
    }
}
