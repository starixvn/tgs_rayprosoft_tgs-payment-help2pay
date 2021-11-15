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
    private readonly Help2PayClient _help2PayClient;
    private static readonly TimeSpan _TimeSpan = TimeSpan.FromHours(8);

    public DepositProcessor(
        ILogger<DepositProcessor> logger,
        IOptions<Help2PayOptions> options,
        Help2PaySerializer help2PaySerializer,
        Help2PayClient help2PayClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _help2PaySerializer = help2PaySerializer ?? throw new ArgumentNullException(nameof(help2PaySerializer));
        _help2PayClient = help2PayClient ?? throw new ArgumentNullException(nameof(help2PayClient));
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
            CallbackUrl = $"{_options.CallbackBaseUrl}/deposit/callback"
        };

        var response = await _help2PayClient
            .DepositAsync(
                settings.MerchantId,
                settings.SecurityCode,
                order,
                string.Empty)
            .ConfigureAwait(false);

        var applyInfo = new Help2PayDepositApplyInfo(settings, response);

        var msg = new DepositResponse
        {
            Result = _help2PaySerializer.Serialize(applyInfo)
        };

        return msg.ToByteArray();
    }
}