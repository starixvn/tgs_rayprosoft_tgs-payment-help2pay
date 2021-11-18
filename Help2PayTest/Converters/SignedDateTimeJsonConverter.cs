using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Help2PayTest.Converters;

internal class SignedDateTimeJsonConverter : JsonConverter<DateTime>
{
    private readonly string _format = "yyyyMMddHHmmss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(
            value.ToString(_format, CultureInfo.InvariantCulture));
}
