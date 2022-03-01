using System;

namespace Tgs.Payment.Help2Pay;

public record Help2PayDepositSettings(
    string MerchantId,
    string SecurityCode,
    string Currency,
    string BankCode,
    Guid TransactionId,
    string ResponseUrl,
    string ClientIp,
    decimal ExchangeRate);
