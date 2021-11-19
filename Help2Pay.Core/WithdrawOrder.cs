namespace Tgs.Payment.Help2Pay;

public record WithdrawOrder
{
    public decimal Amount { get; init; } = default!;

    public string BackAccount { get; init; } = default!;

    public string BankAccountName { get; init; } = default!;

    public string BankCode { get; init; } = default!;

    public string CallbackUrl { get; init; } = default!;

    public string ClientIp { get; init; } = default!;

    public string Currency { get; init; } = default!;

    public string PlayerCode { get; init; } = default!;

    public Guid TransactionId { get; init; } = default!;

    public DateTime TransactionTime { get; init; } = default!;
}
