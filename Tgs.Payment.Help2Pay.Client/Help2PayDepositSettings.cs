using System;

namespace Tgs.Payment.Help2Pay
{
    public record Help2PayDepositSettings(
        string MerchantId,
        string PlayerCode,
        string SecurityCode,
        string Currency,
        string BankCode,
        Guid TransactionId,
        string ResponseUrl,
        string ClientIp);
}
