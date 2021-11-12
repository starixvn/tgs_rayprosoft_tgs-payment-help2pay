using System.Text.Json;

namespace Tgs.Payment.Help2Pay;

public class Help2PayOptions
{
    public string CallbackBaseUrl { get; set; } = default!;

    public string NatsUrl { get; set; } = default!;

    public Uri PaymentApiUrl { get; set; } = default!;

    public JsonSerializerOptions? SystemSerializerOptions { get; set; }

    public Uri PaymentServiceBaseUrl { get; set; } = default!;
}