using System.Text.Json.Serialization;

namespace Tgs.Payment.Help2Pay;

internal record WithdrawHasherInfo
{
    public string MemberCode { get; init; } = default!;

    public string CurrencyCode { get; init; } = default!;

    public decimal Amount { get; init; }

    [JsonPropertyName("TransactionID")]
    public Guid TransactionId { get; init; }

    [JsonPropertyName("TransactionDateTime")]
    public DateTime TransactionTime { get; init; } = default!;

    [JsonPropertyName("ToBankAccountNumber")]
    public string BankAccount { get; init; } = default!;
}
