using System;

namespace Tgs.Payment.Help2Pay
{
    public record Help2PayWithdrawSettings(
        string MerchantId,
        string SecurityCode,
        Guid TransactionId,
        string Currency,
        string BankAccount,
        string BankAccountName,
        string BankCode,
        decimal ExchangeRate);
}
