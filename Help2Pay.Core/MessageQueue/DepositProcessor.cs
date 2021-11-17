using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.MessageQueue;
using Tgs.Payment.Protos;

namespace Tgs.Payment.Help2Pay.MessageQueue;

internal class DepositProcessor : IMessageProcessor
{
    private readonly ILogger<DepositProcessor> _logger;
    private readonly Help2PayOptions _options;
    private readonly Help2PaySerializer _help2PaySerializer;
    private readonly Help2PayDepositClient _help2PayDepositClient;
    private readonly Help2PayQueryClient _help2PayQueryClient;

    private static readonly TimeSpan _TimeSpan = TimeSpan.FromHours(8);

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
            TransactionTime = DateTimeOffset.UtcNow.ToOffset(_TimeSpan).DateTime,
            ResponseUrl = settings.ResponseUrl,
            CallbackUrl = $"{_options.CallbackBaseUrl}/api/deposit/callback"
        };

        var html = await _help2PayDepositClient
            .DepositAsync(
                settings.MerchantId,
                settings.SecurityCode,
                order,
                string.Empty)
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