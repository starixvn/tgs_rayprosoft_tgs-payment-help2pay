using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.MessageQueue;
using Tgs.Payment.Protos;

namespace Tgs.Payment.Help2Pay.MessageQueue;

internal class DepositProcessor : IMessageProcessor
{
    private readonly Help2PayDepositClient _help2PayDepositClient;
    private readonly Help2PayQueryClient _help2PayQueryClient;
    private readonly Help2PaySerializer _help2PaySerializer;
    private readonly ILogger<DepositProcessor> _logger;
    private readonly Help2PayOptions _options;

    public DepositProcessor(
        ILogger<DepositProcessor> logger,
        IOptions<Help2PayOptions> options,
        Help2PaySerializer help2PaySerializer,
        Help2PayDepositClient help2PayDepositClient,
        Help2PayQueryClient help2PayQueryClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _help2PaySerializer = help2PaySerializer ?? throw new ArgumentNullException(nameof(help2PaySerializer));
        _help2PayDepositClient = help2PayDepositClient ?? throw new ArgumentNullException(nameof(help2PayDepositClient));
        _help2PayQueryClient = help2PayQueryClient ?? throw new ArgumentNullException(nameof(help2PayQueryClient));
    }

    public async ValueTask<byte[]> HandleAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var request = DepositRequest.Parser.ParseFrom(data);
        var settings = _help2PaySerializer.Deserialize<Help2PayDepositSettings>(request.ApplyInfo)!;

        var order = new DepositOrder
        {
            PlayerCode = settings.PlayerCode,
            Amount = decimal.Parse(request.Amount),
            Currency = settings.Currency,
            BankCode = settings.BankCode,
            TransactionId = settings.TransactionId,
            TransactionTime = DateTime.UtcNow,
            ResponseUrl = settings.ResponseUrl,
            CallbackUrl = $"{_options.CallbackBaseUrl}/deposit/callback",
            ClientIp = settings.ClientIp
        };

        var html = await _help2PayDepositClient
            .DepositAsync(
                settings.MerchantId,
                settings.SecurityCode,
                order)
            .ConfigureAwait(false);

        var paymentDepositId = await _help2PayQueryClient
            .QueryPaymentDepositIdAsync(settings.MerchantId, settings.TransactionId)
            .ConfigureAwait(false);

        var applyInfo = new Help2PayDepositApplyInfo(settings, new Help2PayDepositResponse(paymentDepositId, html));

        var msg = new DepositResponse
        {
            Result = _help2PaySerializer.Serialize(applyInfo)
        };

        return msg.ToByteArray();
    }
}
