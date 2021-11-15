using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tgs.Payment.Help2Pay.Converters;

namespace Tgs.Payment.Help2Pay;

public class Help2PayHasher
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        WriteIndented = false,
        Converters =
        {
            new SignedDateTimeJsonConverter(),
            new DecimalJsonConverter()
        }
    };

    private readonly Dictionary<string, int> _weight = new Dictionary<string, int>
    {
        ["Reference"] = 1,
        ["TransactionID"] = 1,
        ["Customer"] = 2,
        ["MemberCode"] = 2,
        ["Amount"] = 3,
        ["CurrencyCode"] = 4,
        ["Currency"] = 4,
        ["Datetime"] = 5,
        ["TransactionDateTime"] = 5,
        ["ToBankAccountNumber"] = 6,
        ["Status"] = 7
    };

    //public string ComputeHash(string merchantCode, string securityCode, IDictionary<string, object> dict, string? ip)
    //{
    //    var content = string.Join(
    //        "",
    //        dict.OrderBy(p => _weight[p.Key])
    //        .Select(p => p.Value switch
    //        {
    //            DateTimeOffset => $"{p.Value:yyyyMMddHHmmss}",
    //            decimal => $"{p.Value:f2}",
    //            _ => $"{p.Value ?? "null"}"
    //        }));

    //    var data = $"{merchantCode}{content}{securityCode}{ip}";

    //    //var hash = string.Join(
    //    //    "",
    //    //    MD5.HashData(Encoding.UTF8.GetBytes(data)).Select(b => b.ToString("x2")));

    //    var hash = new BigInteger(MD5.HashData(Encoding.UTF8.GetBytes(data)), isBigEndian: true).ToString("x");

    //    return hash;
    //}

    public string ComputeHash<T>(string merchantCode, string securityCode, T data, string? ip)
    {
        var json = JsonSerializer.Serialize(data, _jsonSerializerOptions);
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;

        var content = string.Join(
            "",
            dict.OrderBy(p => _weight[p.Key])
            .Select(p => p.Value ?? "null"));

        var value = $"{merchantCode}{content}{securityCode}{ip}";

        var hash = new BigInteger(MD5.HashData(Encoding.UTF8.GetBytes(value)), isBigEndian: true).ToString("x");

        return hash;
    }
}