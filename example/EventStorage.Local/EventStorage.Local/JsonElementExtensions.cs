using System;
using System.Buffers;
using System.Text.Json;

namespace EventStorage.Local
{
    internal static class JsonElementExtensions
    {
        /// <summary>Deserializes a <see cref="JsonElement"/> to a desired <see cref="Type"/>.</summary>
        public static object Deserialize(this JsonElement element, Type returnType, JsonSerializerOptions options)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter))
            {
                element.WriteTo(writer);
            }
            return JsonSerializer.Deserialize(bufferWriter.WrittenSpan, returnType, options);
        }
    }
}
