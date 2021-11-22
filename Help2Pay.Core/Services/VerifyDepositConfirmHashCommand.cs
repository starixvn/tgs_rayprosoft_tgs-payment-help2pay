namespace Tgs.Payment.Help2Pay.Services;

public record VerifyDepositConfirmHashCommand(
    string MerchantId,
    string SecurityCode,
    string PlayerCode,
    string Currency,
    decimal Amount,
    Guid ReferenceId,
    string StatusCode,
    string HashKey);
