using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Help2PayTest.Converters;

namespace Help2PayTest;

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

    public string ComputeHash<T>(string merchantId, string securityCode, T data, string? ip = null)
    {
        var json = JsonSerializer.Serialize(data, _jsonSerializerOptions);
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;

        var content = string.Join(
            "",
            dict.OrderBy(p => _weight[p.Key])
            .Select(p => p.Value));

        var value = $"{merchantId}{content}{securityCode}{ip}";

        var hash = new BigInteger(MD5.HashData(Encoding.UTF8.GetBytes(value)), isBigEndian: true).ToString("x");

        return hash;
    }
}
