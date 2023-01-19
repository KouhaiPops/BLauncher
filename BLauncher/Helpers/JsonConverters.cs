using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLauncher.Helpers
{
    public sealed class JsonConverters
    {
        public sealed class StringKVPConverter : JsonConverter<Dictionary<string, string>>
        {
            public override Dictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                Dictionary<string, string> dict = new();
                if(reader.TokenType == JsonTokenType.StartArray)
                {
                    reader.Read();
                    while(reader.TokenType != JsonTokenType.EndArray)
                    {
                        // Read start object
                        reader.Read();
                        // Get the string
                        var key = reader.GetString();
                        // Advance the reader
                        reader.Read();
                        var value = reader.GetString();
                        // Advance reader
                        reader.Read();
                        // Deserialize
                        if(!string.IsNullOrWhiteSpace(key))
                        {
                            dict[key] = value;
                        }
                        // Read end of object
                        reader.Read();
                    }
                }
                return dict;
            }

            public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
            {
                writer.WriteStartArray();
                foreach(var stringKVP in value)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(stringKVP.Key);
                    writer.WriteStringValue(stringKVP.Value);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }
        }
    }
}
