using System.Text.Json;

namespace Tgs.Payment.Help2Pay;

public class Help2PaySerializer
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public T? Deserialize<T>(string json)
        => JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);

    public string Serialize<T>(T data)
        => JsonSerializer.Serialize(data, _jsonSerializerOptions);
}