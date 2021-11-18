namespace Tgs.Payment.Help2Pay;

public record DepositOrder
{
    public string Currency { get; init; } = default!;

    public string PlayerCode { get; init; } = default!;

    public decimal Amount { get; init; }

    public Guid TransactionId { get; init; }

    public DateTime TransactionTime { get; init; }

    public string ResponseUrl { get; init; } = default!;

    public string CallbackUrl { get; init; } = default!;

    public string BankCode { get; init; } = default!;
}
