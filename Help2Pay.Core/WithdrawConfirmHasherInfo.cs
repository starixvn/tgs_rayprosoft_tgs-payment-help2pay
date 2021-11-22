namespace Tgs.Payment.Help2Pay;

public record WithdrawConfirmHasherInfo(
    string MemberCode,
    string CurrencyCode,
    decimal Amount,
    Guid TransactionId,
    string Status);
