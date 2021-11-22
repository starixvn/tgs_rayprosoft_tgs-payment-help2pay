namespace Tgs.Payment.Help2Pay.Services;

public record VerifyWithdrawConfirmHashCommand(
    string MerchantId,
    string SecurityCode,
    string PlayerCode,
    string Currency,
    decimal Amount,
    Guid TransactionId,
    string StatusCode,
    string HashKey);
