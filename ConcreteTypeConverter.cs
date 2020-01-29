﻿using Newtonsoft.Json;
using System;

namespace LastIRead {
    /// <summary>
    /// Concrete type converter.
    /// Based on https://stackoverflow.com/a/12202914/2422905.
    /// </summary>
    /// <typeparam name="TConcrete">Concrete type</typeparam>
    public class ConcreteTypeConverter<TConcrete> : JsonConverter {
        public override bool CanConvert(Type objectType) {
            //assume we can convert to anything for now
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            //explicitly specify the concrete type we want to create
            return serializer.Deserialize<TConcrete>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            //use the default serialization - it works fine
            serializer.Serialize(writer, value);
        }
    }
}
