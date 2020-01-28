using LastIRead.Data.Instance;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LastIRead.Import {
    public class CSVImport : IDataImporter {
        public List<IReadable> Import(FileInfo file) {
            using var readStream = file.OpenText();
            string? line = readStream.ReadLine();

            var columns = line.Split(',').Select(text => text.ToLowerInvariant()).ToList();

            var titleIndex = columns.IndexOf("title");
            var progressIndex = columns.IndexOf("progress");

            if (titleIndex < 0) {
                throw new InvalidFormatException();
            }

            var list = new List<IReadable>();

            while ((line = readStream.ReadLine()) != null) {
                var split = line.Split(',');

                var readable = new GenericReadable {
                    Title = split[titleIndex]
                };

                if (progressIndex >= 0 && int.TryParse(split[progressIndex], out var progress)) {
                    readable.LogProgress(progress);
                }

                list.Add(readable);
            }

            return list;
        }
    }
}
