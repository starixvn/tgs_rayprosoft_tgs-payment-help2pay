namespace Tgs.Payment.Help2Pay.ApplicationServices;

public record ApproveDepositFormCommand(
    Guid FormId,
    Guid BrandId,
    decimal? ActualAmount);
