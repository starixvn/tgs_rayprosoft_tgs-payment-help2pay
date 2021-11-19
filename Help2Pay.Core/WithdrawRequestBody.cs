using System.Text.Json.Serialization;

namespace Tgs.Payment.Help2Pay;

internal class WithdrawRequestBody
{
    [JsonPropertyName("Amount")]
    public decimal Amount { get; init; }

    [JsonPropertyName("toBankAccountNumber")]
    public string BankAccount { get; init; } = default!;

    [JsonPropertyName("toBankAccountName")]
    public string BankAccountName { get; init; } = default!;

    [JsonPropertyName("BankCode")]
    public string BankCode { get; init; } = default!;

    [JsonPropertyName("ReturnURI")]
    public string CallbackUrl { get; init; } = default!;

    [JsonPropertyName("ClientIP")]
    public string ClientIp { get; init; } = default!;

    [JsonPropertyName("CurrencyCode")]
    public string Currency { get; init; } = default!;

    [JsonPropertyName("Key")]
    public string Hash { get; init; } = default!;

    [JsonPropertyName("MerchantCode")]
    public string MerchantId { get; init; } = default!;

    [JsonPropertyName("MemberCode")]
    public string PlayerCode { get; init; } = default!;

    [JsonPropertyName("TransactionID")]
    public Guid TransactionId { get; init; } = default!;

    [JsonPropertyName("TransactionDateTime")]
    public DateTime TransactionTime { get; init; } = default!;
}
