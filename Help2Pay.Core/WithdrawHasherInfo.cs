namespace Tgs.Payment.Help2Pay;

internal record WithdrawHasherInfo(
    string MemberCode,
    string CurrencyCode,
    decimal Amount,
    Guid TransactionID,
    DateTime TransactionDateTime,
    string ToBankAccountNumber);
