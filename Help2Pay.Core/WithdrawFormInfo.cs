using Tgs.Payment.Clients;

namespace Tgs.Payment.Help2Pay;

public record WithdrawFormInfo(
    Guid Id,
    string? UserId,
    UserType UserType,
    Guid BrandId,
    Guid? BrandPaymentId,
    decimal Amount,
    PaymentFlowState Status,
    Guid BankInfoId,
    string? BankInfo,
    Help2PayWithdrawApplyInfo? ApplyInfo,
    string? PaymentReturn = null);