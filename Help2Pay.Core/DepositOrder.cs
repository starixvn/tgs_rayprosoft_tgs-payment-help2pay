using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tgs.Payment.Help2Pay;

public record DepositOrder
{
    [JsonPropertyName("Merchant")]
    public string MerchantId { get; init; }

    [JsonPropertyName("Currency")]
    public string Currency { get; init; }

    [JsonPropertyName("Custormer")]
    public string PlayerCode { get; init; }

    [JsonPropertyName("Amount")]
    public decimal Amount { get; init; }

    [JsonPropertyName("Reference")]
    public Guid TransactionId { get; init; }

    [JsonPropertyName("Datetime")]
    public DateTimeOffset TransactionTime { get; init; }

    [JsonPropertyName("FrontURI")]
    public string ResponseUrl { get; init; }

    [JsonPropertyName("BackcURI")]
    public string CallbackUrl { get; init; }

    [JsonPropertyName("Bank")]
    public string BankCode { get; init; }

    [JsonPropertyName("Language")]
    public string Language { get; init; }

    [JsonPropertyName("ClientIP")]
    public string ClientIp { get; init; }
}