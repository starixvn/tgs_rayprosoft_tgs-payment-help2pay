namespace Tgs.Payment.Help2Pay;

internal record DepositHasherInfo(
    string Customer,
    string Currency,
    decimal Amount,
    Guid Reference,
    DateTime Datetime);
