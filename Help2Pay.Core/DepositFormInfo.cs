using Tgs.Payment.Clients;

namespace Tgs.Payment.Help2Pay;

public record DepositFormInfo(
    Guid Id,
    string PlayerCode,
    Guid BrandId,
    Guid BrandPaymentId,
    decimal Amount,
    PaymentFlowState Status,
    Help2PayDepositApplyInfo? ApplyInfo,
    string? PaymentReturn = null);
