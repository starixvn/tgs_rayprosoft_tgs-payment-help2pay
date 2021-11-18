using System;

namespace Tgs.Payment.Help2Pay
{
    public record Help2PayWithdrawSettings(
        string MerchantId,
        string PlayerCode,
        string SecurityCode,
        Guid TransactionId,
        string Currency,
        string BankAccount,
        string BankAccountName,
        string BankCode);
}
