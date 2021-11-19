using System.Text.Json;

namespace Tgs.Payment.Help2Pay;

public class Help2PayOptions
{
    public string CallbackBaseUrl { get; set; } = default!;

    public string NatsUrl { get; set; } = default!;

    public Uri PaymentDepositApiUrl { get; set; } = default!;

    public Uri PaymentQueryApiUrl { get; set; } = default!;

    public Uri PaymentServiceBaseUrl { get; set; } = default!;

    public Uri PaymentWithdrawApiUrl { get; set; } = default!;

    public JsonSerializerOptions? SystemSerializerOptions { get; set; }
}
