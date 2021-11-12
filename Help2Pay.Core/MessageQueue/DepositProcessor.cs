using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.MessageQueue;
using Tgs.Payment.Protos;

namespace Tgs.Payment.Help2Pay.MessageQueue;

internal class DepositProcessor : IMessageProcessor
{
    private readonly ILogger<DepositProcessor> _logger;
    private readonly IOptions<Help2PayOptions> _options;
    private readonly Help2PaySerializer _help2PaySerializer;

    private static readonly TimeSpan _TimeSpan = TimeSpan.FromHours(8);

    public DepositProcessor(
        ILogger<DepositProcessor> logger,
        IOptions<Help2PayOptions> options,
        Help2PaySerializer help2PaySerializer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _help2PaySerializer = help2PaySerializer ?? throw new ArgumentNullException(nameof(help2PaySerializer));
    }

    public ValueTask<byte[]> HandleAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var request = DepositRequest.Parser.ParseFrom(data);
        var settings = _help2PaySerializer.Deserialize<Help2PayDepositSettings>(request.ApplyInfo)!;

        var order = new DepositOrder
        {
            Amount = decimal.Parse(request.Amount),
            MerchantId = settings.MerchantId,
            Currency = settings.Currency,
            BankCode = settings.BankCode,
            TransactionId = settings.TransactionId,
            TransactionTime = DateTimeOffset.UtcNow.ToOffset(_TimeSpan),
            ResponseUrl = settings.ResponseUrl,
            ClientIp = string.Empty,
        };

        return default;
    }
}