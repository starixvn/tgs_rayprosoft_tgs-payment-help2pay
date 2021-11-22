using System.Text.Json.Serialization;

namespace Tgs.Payment.Help2Pay.ViewModels;

public class DepositConfirmViewModel
{
    [JsonPropertyName("Amount")]
    public decimal Amount { get; init; } = default!;

    [JsonPropertyName("Currency")]
    public string Currency { get; init; } = default!;

    [JsonPropertyName("DepositFee")]
    public decimal DepositFee { get; init; } = default!;

    [JsonPropertyName("ID")]
    public string DepositId { get; init; } = default!;

    [JsonPropertyName("ErrorCode")]
    public string? ErrorCode { get; init; } = default!;

    [JsonPropertyName("Key")]
    public string Hash { get; init; } = default!;

    [JsonPropertyName("Merchant")]
    public string MerchantId { get; init; } = default!;

    [JsonPropertyName("Note")]
    public string? Note { get; init; }

    [JsonPropertyName("Customer")]
    public string PlayerCode { get; init; } = default!;

    [JsonPropertyName("Reference")]
    public Guid ReferenceId { get; init; } = default!;

    [JsonPropertyName("StatementDate")]
    public string StatementTime { get; init; } = default!;

    [JsonPropertyName("Status")]
    public string Status { get; init; } = default!;

    [JsonPropertyName("Datetime")]
    public string TransactionTime { get; init; } = default!;
}
