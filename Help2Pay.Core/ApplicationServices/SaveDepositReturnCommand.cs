using Tgs.Payment.Clients;

namespace Tgs.Payment.Help2Pay.ApplicationServices;

public record SaveDepositReturnCommand(
    Guid FormId,
    PaymentFlowState State,
    Help2PayDepositReturn DepositReturn);
