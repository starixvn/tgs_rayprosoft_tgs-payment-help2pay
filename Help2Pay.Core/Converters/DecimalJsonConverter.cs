using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.Help2Pay.Converters;

internal class DecimalJsonConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (decimal.TryParse(reader.GetString(), out var result))
            return result;

        return default;
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString("f2"));
}