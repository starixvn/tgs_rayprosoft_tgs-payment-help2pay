using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tgs.Payment.Clients;
using Tgs.Payment.Help2Pay;

namespace Help2PayTest;

internal class Demo : BackgroundService
{
    private readonly IPaymentClient _paymentClient;
    private readonly Help2PaySerializer _help2PaySerializer;
    private readonly ILogger<Demo> _logger;

    public Demo(
        IPaymentClient paymentClient,
        Help2PaySerializer help2PaySerializer,
        ILogger<Demo> logger)
    {
        _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
        _help2PaySerializer = help2PaySerializer ?? throw new ArgumentNullException(nameof(help2PaySerializer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = new Help2PayDepositSettings(
                "INT0492",
                "4ucmqplxb029",
                "juJFlG7Jo3yrdeK1Dmeg",
                "PHP",
                "BDO",
                Guid.NewGuid(),
                "http://www.google.com");

        var paymentReturn = await _paymentClient.DepositAsync(
            "tgs.payment.help2pay.deposit",
            100m,
            _help2PaySerializer.Serialize(settings))
            .ConfigureAwait(false);
    }
}