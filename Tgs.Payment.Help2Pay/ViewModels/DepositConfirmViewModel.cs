using Microsoft.AspNetCore.Mvc;

namespace Tgs.Payment.Help2Pay.ViewModels;

public class DepositConfirmViewModel
{
    [FromForm(Name = "Amount")]
    public decimal Amount { get; init; } = default!;

    [FromForm(Name = "Currency")]
    public string Currency { get; init; } = default!;

    [FromForm(Name = "DepositFee")]
    public decimal DepositFee { get; init; } = default!;

    [FromForm(Name = "ID")]
    public string DepositId { get; init; } = default!;

    [FromForm(Name = "ErrorCode")]
    public string? ErrorCode { get; init; } = default!;

    [FromForm(Name = "Key")]
    public string Hash { get; init; } = default!;

    [FromForm(Name = "Merchant")]
    public string MerchantId { get; init; } = default!;

    [FromForm(Name = "Note")]
    public string? Note { get; init; }

    [FromForm(Name = "Customer")]
    public string PlayerCode { get; init; } = default!;

    [FromForm(Name = "Reference")]
    public Guid ReferenceId { get; init; } = default!;

    [FromForm(Name = "StatementDate")]
    public string StatementTime { get; init; } = default!;

    [FromForm(Name = "Status")]
    public string Status { get; init; } = default!;

    [FromForm(Name = "Datetime")]
    public string TransactionTime { get; init; } = default!;
}
