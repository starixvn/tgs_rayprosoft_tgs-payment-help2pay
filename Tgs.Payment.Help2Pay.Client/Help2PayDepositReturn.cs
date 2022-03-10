using System;

namespace Tgs.Payment.Help2Pay;

public record Help2PayDepositReturn(
    string MerchantId,
    string DepositId,
    string PlayerCode,
    string Currency,
    Guid ReferenceId,
    decimal Amount,
    decimal DepositFee,
    string TransactionTime,
    string StatementTime,
    string StatusCode,
    string? ErrorCode,
    string? Note);
