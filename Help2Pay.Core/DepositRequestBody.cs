using System.Text.Json.Serialization;

namespace Tgs.Payment.Help2Pay;

internal class DepositRequestBody
{
    [JsonPropertyName("Amount")]
    public decimal Amount { get; init; }

    [JsonPropertyName("Bank")]
    public string BankCode { get; init; } = default!;

    [JsonPropertyName("BackURI")]
    public string CallbackUrl { get; init; } = default!;

    [JsonPropertyName("ClientIP")]
    public string? ClientIp { get; init; }

    [JsonPropertyName("Currency")]
    public string Currency { get; init; } = default!;

    [JsonPropertyName("Key")]
    public string Hash { get; init; } = default!;

    [JsonPropertyName("Language")]
    public string Language { get; init; } = default!;

    [JsonPropertyName("Merchant")]
    public string MerchantId { get; init; } = default!;

    [JsonPropertyName("Customer")]
    public string PlayerCode { get; init; } = default!;

    [JsonPropertyName("Reference")]
    public Guid ReferenceId { get; init; }

    [JsonPropertyName("FrontURI")]
    public string ResponseUrl { get; init; } = default!;

    [JsonPropertyName("Datetime")]
    public DateTime TransactionTime { get; init; }
}
