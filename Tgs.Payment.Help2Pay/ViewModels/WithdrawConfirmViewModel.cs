using System.Text.Json.Serialization;

namespace Tgs.Payment.Help2Pay.ViewModels;

public class WithdrawConfirmViewModel
{
    [JsonPropertyName("Amount")]
    public decimal Amount { get; init; }

    [JsonPropertyName("CurrencyCode")]
    public string Currency { get; init; } = default!;

    [JsonPropertyName("Key")]
    public string Hash { get; init; } = default!;

    [JsonPropertyName("MerchantCode")]
    public string MerchantId { get; init; } = default!;

    [JsonPropertyName("Message")]
    public string? Message { get; init; }

    [JsonPropertyName("PayoutFee")]
    public decimal PayoutFee { get; init; }

    [JsonPropertyName("MemberCode")]
    public string PlayerCode { get; init; } = default!;

    [JsonPropertyName("Status")]
    public string Status { get; init; } = default!;

    [JsonPropertyName("TransactionID")]
    public Guid TransactionId { get; init; }

    [JsonPropertyName("TransactionDatetime")]
    public string TransactionTime { get; init; } = default!;

    [JsonPropertyName("ID")]
    public string WithdrawId { get; init; } = default!;
}
