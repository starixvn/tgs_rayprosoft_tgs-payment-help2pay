using Microsoft.Extensions.Hosting;
using Tgs.Payment.Clients;
using Tgs.Payment.Help2Pay;

namespace Help2PayTest;

internal class Demo : BackgroundService
{
    private readonly Help2PaySerializer _help2PaySerializer;
    private readonly IPaymentClient _paymentClient;

    public Demo(
        IPaymentClient paymentClient,
        Help2PaySerializer help2PaySerializer)
    {
        _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
        _help2PaySerializer = help2PaySerializer ?? throw new ArgumentNullException(nameof(help2PaySerializer));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //await DepositAsync().ConfigureAwait(false);

        await WithdrawAsync().ConfigureAwait(false);
    }

#pragma warning disable IDE0051 // 刪除未使用的私用成員

    private async Task DepositAsync()
#pragma warning restore IDE0051 // 刪除未使用的私用成員
    {
        var settings = new Help2PayDepositSettings(
                        "INT0492",
                        "juJFlG7Jo3yrdeK1Dmeg",
                        "PHP",
                        "BDO",
                        Guid.NewGuid(),
                        "http://www.google.com",
                        "127.0.0.1",
                        1m);

        var paymentReturn = await _paymentClient.DepositAsync(
            "tgs.payment.help2pay.deposit",
            100m,
            _help2PaySerializer.Serialize(settings))
            .ConfigureAwait(false);

        Console.WriteLine(paymentReturn);
    }

    private async Task WithdrawAsync()
    {
        var settings = new Help2PayWithdrawSettings(
                       "INT0492",
                       "juJFlG7Jo3yrdeK1Dmeg",
                       Guid.NewGuid(),
                       "PHP",
                       "1213015890",
                       "steave",
                       "BDO",
                       1m);

        var withdrawReturn = await _paymentClient.WithdrawAsync(
            "tgs.payment.help2pay.withdraw",
            130m,
            _help2PaySerializer.Serialize(settings))
            .ConfigureAwait(false);

        Console.WriteLine(withdrawReturn);
    }
}
