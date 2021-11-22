using System.Text.Json;
using System.Xml.Serialization;
using Help2PayTest.Converters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tgs.Payment.Clients;
using Tgs.Payment.Help2Pay;

namespace Help2PayTest;

internal class Demo : BackgroundService
{
    private readonly Help2PaySerializer _help2PaySerializer;
    private readonly ILogger<Demo> _logger;
    private readonly IPaymentClient _paymentClient;

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
        //await DepositAsync().ConfigureAwait(false);

        //await QueryDepositFormAsync().ConfigureAwait(false);

        await WithdrawAsync().ConfigureAwait(false);
    }

    private async Task DepositAsync()
    {
        var settings = new Help2PayDepositSettings(
                        "INT0492",
                        "4ucmqplxb029",
                        "juJFlG7Jo3yrdeK1Dmeg",
                        "PHP",
                        "BDO",
                        Guid.NewGuid(),
                        "http://www.google.com",
                        "127.0.0.1");

        var paymentReturn = await _paymentClient.DepositAsync(
            "tgs.payment.help2pay.deposit",
            100m,
            _help2PaySerializer.Serialize(settings))
            .ConfigureAwait(false);

        Console.WriteLine(paymentReturn);
    }

    private async Task QueryDepositFormAsync()
    {
        var merchant = "INT0492";
        var referenceId = Guid.Parse("d2077f92-b074-4ca4-a4f8-09cedf5b82aa");

        var url = $"https://query.testingzone88.com/Services/Merchants/{merchant}/TransferStatus/{referenceId}";

        using var httpClient = new HttpClient();

        var response = await httpClient.PostAsync(url, null).ConfigureAwait(false);

        var value = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        Console.WriteLine("status code: {0}", response.StatusCode);
        Console.WriteLine("data: {0}", value);

        try
        {
            using var sr = new StringReader(value);
            var serializer = new XmlSerializer(typeof(WithdrawResponse));
            var model = serializer.Deserialize(sr);

            Console.WriteLine(JsonSerializer.Serialize(model));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, string.Empty);
        }
    }

    private async Task WithdrawAsync()
    {
        var settings = new Help2PayWithdrawSettings(
                       "INT0492",
                       "4ucmqplxb029",
                       "juJFlG7Jo3yrdeK1Dmeg",
                       Guid.NewGuid(),
                       "PHP",
                       "1213015890",
                       "steave",
                       "BDO");

        var withdrawReturn = await _paymentClient.WithdrawAsync(
            "tgs.payment.help2pay.withdraw",
            130m,
            _help2PaySerializer.Serialize(settings))
            .ConfigureAwait(false);

        Console.WriteLine(withdrawReturn);
    }
}
