using System;

namespace Tgs.Payment.Help2Pay;

public record Help2PayWithdrawReturn(
    string MerchantId,
    string WithdrawId,
    string PlayerCode,
    string Currency,
    Guid TransactionId,
    decimal Amount,
    decimal WithdrawFee,
    string TransactionTime,
    string StatusCode,
    string? Message);
