namespace Tgs.Payment.Help2Pay;

public record WithdrawOrder
{
    public string Currency { get; init; }

    public string PlayerCode { get; init; }

    public decimal Amount { get; init; }

    public Guid TransactionId { get; init; }

    public DateTime TransactionTime { get; init; }

    public string CallbackUrl { get; init; }

    public string BankCode { get; init; }

    public string BackAccount { get; init; }

    public string BankAccountName { get; init; }
}
