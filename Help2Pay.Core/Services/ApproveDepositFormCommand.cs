namespace Tgs.Payment.Help2Pay.Services;

public record ApproveDepositFormCommand(
    Guid FormId,
    Guid BrandId,
    decimal? ActualAmount);