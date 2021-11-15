using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Tgs.Payment.Help2Pay.Converters;

namespace Tgs.Payment.Help2Pay;

internal record DepositRequestBody
{
    [JsonPropertyName("Merchant")]
    public string MerchantId { get; init; } = default!;

    [JsonPropertyName("Reference")]
    public Guid TransactionId { get; init; }

    [JsonPropertyName("Customer")]
    public string PlayerCode { get; init; } = default!;

    [JsonPropertyName("Amount")]
    [JsonConverter(typeof(DecimalJsonConverter))]
    public decimal Amount { get; init; }

    [JsonPropertyName("Currency")]
    public string Currency { get; init; } = default!;

    [JsonPropertyName("Datetime")]
    public DateTime TransactionTime { get; init; }

    [JsonPropertyName("Key")]
    public string Hash { get; init; } = default!;

    [JsonPropertyName("FrontURI")]
    public string ResponseUrl { get; init; } = default!;

    [JsonPropertyName("BackURI")]
    public string CallbackUrl { get; init; } = default!;

    [JsonPropertyName("Bank")]
    public string BankCode { get; init; } = default!;

    [JsonPropertyName("Language")]
    public string Language { get; init; } = default!;

    [JsonPropertyName("ClientIP")]
    public string? ClientIp { get; init; }
}