using Microsoft.Extensions.Logging;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.Help2Pay.ApplicationServices;

internal class SaveWithdrawReturnCommandService : ICommandService<SaveWithdrawReturnCommand>
{
    private readonly Help2PaySerializer _help2PaySerializer;
    private readonly ILogger<SaveWithdrawReturnCommandService> _logger;
    private readonly IPaymentServiceClient _paymentServiceClient;

    public SaveWithdrawReturnCommandService(
        ILogger<SaveWithdrawReturnCommandService> logger,
        IPaymentServiceClient paymentServiceClient,
        Help2PaySerializer help2PaySerializer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        _help2PaySerializer = help2PaySerializer ?? throw new ArgumentNullException(nameof(help2PaySerializer));
    }

    public ValueTask ExecuteAsync(SaveWithdrawReturnCommand command, CancellationToken cancellationToken = default)
        => _paymentServiceClient.SubmitWithdrawFormStatusAsync(
            command.FormId,
            command.State,
            _help2PaySerializer.Serialize(command.WithdrawReturn),
            command.ActualAmount);
}
