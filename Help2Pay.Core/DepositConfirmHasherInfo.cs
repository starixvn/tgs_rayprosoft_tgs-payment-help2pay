namespace Tgs.Payment.Help2Pay;

public record DepositConfirmHasherInfo(
    string Customer,
    string Currency,
    decimal Amount,
    Guid Reference,
    string Status);