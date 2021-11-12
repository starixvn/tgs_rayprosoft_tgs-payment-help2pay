using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.Help2Pay.Converters;

internal class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    private readonly string _Format = "yyyyMMddHHmmss";

    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTimeOffset.ParseExact(reader.GetString()!, _Format, null, DateTimeStyles.AssumeLocal);

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        => writer.WriteStringValue(
            value.ToString(_Format, CultureInfo.InvariantCulture));
}