using Tgs.Payment.Clients;

namespace Tgs.Payment.Help2Pay.ApplicationServices;

public record SaveWithdrawReturnCommand(
    Guid FormId,
    decimal? ActualAmount,
    Help2PayWithdrawReturn WithdrawReturn);
