using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.MessageQueue;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;

namespace Tgs.Payment.Help2Pay.MessageQueue;

internal class WithdrawProcessor : IMessageProcessor
{
    private readonly Help2PayQueryClient _help2PayQueryClient;
    private readonly Help2PaySerializer _help2PaySerializer;
    private readonly Help2PayWithdrawClient _help2PayWithdrawClient;
    private readonly ILogger<WithdrawProcessor> _logger;
    private readonly Help2PayOptions _options;
    private readonly IPaymentClient _paymentClient;

    public WithdrawProcessor(
        ILogger<WithdrawProcessor> logger,
        IOptions<Help2PayOptions> options,
        Help2PaySerializer help2PaySerializer,
        Help2PayWithdrawClient help2PayWithdrawClient,
        Help2PayQueryClient help2PayQueryClient,
        IPaymentClient paymentClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _help2PaySerializer = help2PaySerializer ?? throw new ArgumentNullException(nameof(help2PaySerializer));
        _help2PayWithdrawClient = help2PayWithdrawClient ?? throw new ArgumentNullException(nameof(help2PayWithdrawClient));
        _help2PayQueryClient = help2PayQueryClient ?? throw new ArgumentNullException(nameof(help2PayQueryClient));
        _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
    }

    public async ValueTask<byte[]> HandleAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var request = WithdrawRequest.Parser.ParseFrom(data);
        var settings = _help2PaySerializer.Deserialize<Help2PayWithdrawSettings>(request.ApplyInfo)!;

        var order = new WithdrawOrder
        {
            PlayerCode = settings.PlayerCode,
            Amount = decimal.Parse(request.Amount),
            Currency = settings.Currency,
            BankCode = settings.BankCode,
            TransactionId = settings.TransactionId,
            TransactionTime = DateTime.UtcNow,
            CallbackUrl = $"{_options.CallbackBaseUrl}/api/withdraw/callback",
            BackAccount = settings.BankAccount,
            BankAccountName = settings.BankAccountName,
            ClientIp = "127.0.0.1"
        };

        var withdrawResponse = await _help2PayWithdrawClient.WithdrawAsync(settings.MerchantId, settings.SecurityCode, order)
            .ConfigureAwait(false);

        if (withdrawResponse.StatusCode != "000")
        {
            throw new ApiException($"Create withdraw form failed. StatusCode: {withdrawResponse.StatusCode}, Reason: {withdrawResponse.Message}");
        }

        var paymentWithdrawId = await _help2PayQueryClient
           .QueryPaymentWithdrawIdAsync(settings.MerchantId, settings.TransactionId)
           .ConfigureAwait(false);

        var applyInfo = new Help2PayWithdrawApplyInfo(
            settings,
            new Help2PayWithdrawResponse(
                paymentWithdrawId,
                withdrawResponse.StatusCode,
                withdrawResponse.Message));

        await _paymentClient
            .SaveWithdrawFormThirdPartyOrderNoAsync(settings.TransactionId, applyInfo.Response.PaymentWithdrawId)
            .ConfigureAwait(false);

        var msg = new WithdrawResponse
        {
            Result = _help2PaySerializer.Serialize(applyInfo)
        };

        return msg.ToByteArray();
    }
}
