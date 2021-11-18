using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Help2PayTest.Converters;

internal class TwelveHourDateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(
            value.ToString("yyyy-MM-dd hh:mm:sstt", CultureInfo.InvariantCulture));
}
