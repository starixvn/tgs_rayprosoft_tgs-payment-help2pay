using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Tgs.Payment.Help2Pay.Converters;

namespace Tgs.Payment.Help2Pay;

internal record DepositHasherInfo
{
    public string MerchantId { get; init; }

    public string TransactionId { get; init; }

    public string PlayerCode { get; init; }

    [JsonConverter(typeof(DecimalJsonConverter))]
    public decimal Amount { get; init; }

    public string Currency { get; init; }

    [JsonConverter(typeof(DateTimeOffsetJsonConverter))]
    public DateTimeOffset TransactionTime { get; init; }

    public string SecurityCode { get; init; }

    public string ClientIp { get; init; }
}