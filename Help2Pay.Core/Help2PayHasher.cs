using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Tgs.Payment.Help2Pay;

public class Help2PayHasher
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        WriteIndented = false
    };

    public string ComputeHash<T>(T info)
    {
        //var merchant = parameters.ContainsKey("Merchant") ? $"{parameters["Merchant"]}" : string.Empty;
        //var reference = parameters.ContainsKey("Reference") ? $"{parameters["Reference"]}" : string.Empty;
        //var customer = parameters.ContainsKey("Customer") ? $"{parameters["Customer"]}" : string.Empty;
        //var amount = parameters.ContainsKey("Amount") ? $"{(decimal)parameters["Amount"]:f2}" : string.Empty;
        //var currency = parameters.ContainsKey("Currency") ? $"{parameters["Currency"]}" : string.Empty;
        //var datetime = parameters.ContainsKey("DateTime") ? $"{(DateTimeOffset)parameters["DateTime"]:yyyyMMddHHmmss}" : string.Empty;
        //var clientIp = parameters.ContainsKey("ClientIP") ? $"{parameters["ClientIP"]}" : string.Empty;

        var values = JsonSerializer.Serialize(info, _jsonSerializerOptions);
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(values, _jsonSerializerOptions)!;

        var data = string.Join(string.Empty, dict.Values);
        var hash = string.Join(
            string.Empty,
            MD5.HashData(Encoding.UTF8.GetBytes(data)).Select(b => b.ToString("x2")));

        return hash;
    }
}