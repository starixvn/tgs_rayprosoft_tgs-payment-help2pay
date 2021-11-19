using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.Help2Pay.Converters;

internal class SignedDateTimeJsonConverter : JsonConverter<DateTime>
{
    private readonly string _format = "yyyyMMddHHmmss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(
            new DateTimeOffset(value).ToOffset(TimeSpan.FromHours(8D)).DateTime.ToString(_format, CultureInfo.InvariantCulture));
}
