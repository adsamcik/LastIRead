using LastIRead.Data.Instance;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LastIRead.Import.Implementation {
    /// <summary>
    /// JSON data handler providing JSON import and export for IReadables.
    /// </summary>
    class JSONDataHandler : IDataImporter, IDataExporter {
        public string[] ImportExtensions => new string[] { "json" };

        public string[] ExportExtensions => ImportExtensions;

        private JsonSerializer Serializer => new JsonSerializer();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        public async Task Export(IList<IReadable> readables, FileInfo file) {
            using StreamWriter writer = File.CreateText(file.FullName);

            Serializer.Serialize(writer, readables);
        }

        public async Task<IList<IReadable>> Import(FileInfo file) {
            using StreamReader reader = new StreamReader(file.FullName);

            var serialize = Serializer;

            serialize.Converters.Add(new ProgressConverter());
            serialize.Converters.Add(new ProgressArrayConverter());
            var list = (List<GenericReadable>)serialize.Deserialize(reader, typeof(List<GenericReadable>));
            return list.Cast<IReadable>().ToList();
        }

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        /// <summary>
        /// Progress converter.
        /// For now converts any progress to GenericProgress on serialization.
        /// </summary>
        class ProgressConverter : JsonConverter {
            public override bool CanConvert(Type objectType) {
                return objectType == typeof(IProgress);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
                return serializer.Deserialize<GenericProgress>(reader);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
                serializer.Serialize(writer, value);
            }
        }

        /// <summary>
        /// Progress array converter.
        /// Required for proper progress list serialization.
        /// </summary>
        class ProgressArrayConverter : JsonConverter {
            public override bool CanConvert(Type objectType) {
                return (objectType == typeof(List<IProgress>));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
                var array = JArray.Load(reader);
                var list = new List<IProgress>();
                foreach (var item in array) {
                    list.Add(item.ToObject<IProgress>(serializer));
                }
                return list;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
                serializer.Serialize(writer, value);
            }

            public override bool CanWrite {
                get { return true; }
            }
        }
    }
}
