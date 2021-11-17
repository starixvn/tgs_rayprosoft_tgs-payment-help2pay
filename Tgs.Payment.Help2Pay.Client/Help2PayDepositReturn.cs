namespace Tgs.Payment.Help2Pay;

public record Help2PayDepositReturn(
    string DepositId,
    string PlayerCode,
    Guid ReferenceId,
    decimal Amount,
    decimal DepositFee,
    string TransactionTime,
    string StatementTime,
    string StatusCode,
    string? ErrorCode,
    string? Note);