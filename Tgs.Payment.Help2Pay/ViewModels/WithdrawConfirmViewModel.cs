using Microsoft.AspNetCore.Mvc;

namespace Tgs.Payment.Help2Pay.ViewModels;

public class WithdrawConfirmViewModel
{
    [FromForm(Name = "Amount")]
    public decimal Amount { get; init; }

    [FromForm(Name = "CurrencyCode")]
    public string Currency { get; init; } = default!;

    [FromForm(Name = "Key")]
    public string Hash { get; init; } = default!;

    [FromForm(Name = "MerchantCode")]
    public string MerchantId { get; init; } = default!;

    [FromForm(Name = "Message")]
    public string? Message { get; init; }

    [FromForm(Name = "PayoutFee")]
    public decimal PayoutFee { get; init; }

    [FromForm(Name = "MemberCode")]
    public string PlayerCode { get; init; } = default!;

    [FromForm(Name = "Status")]
    public string Status { get; init; } = default!;

    [FromForm(Name = "TransactionID")]
    public Guid TransactionId { get; init; }

    [FromForm(Name = "TransactionDatetime")]
    public string TransactionTime { get; init; } = default!;

    [FromForm(Name = "ID")]
    public string WithdrawId { get; init; } = default!;
}
