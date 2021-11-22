using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.Help2Pay.ApplicationServices;

internal class SaveDepositReturnCommandService : ICommandService<SaveDepositReturnCommand>
{
    private readonly Help2PaySerializer _help2PaySerializer;
    private readonly IPaymentServiceClient _paymentServiceClient;

    public SaveDepositReturnCommandService(
        IPaymentServiceClient paymentServiceClient,
        Help2PaySerializer help2PaySerializer)
    {
        _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        _help2PaySerializer = help2PaySerializer ?? throw new ArgumentNullException(nameof(help2PaySerializer));
    }

    public ValueTask ExecuteAsync(SaveDepositReturnCommand command, CancellationToken cancellationToken = default)
        => _paymentServiceClient.SubmitDepositFormStatusAsync(
                command.FormId,
                command.State,
                _help2PaySerializer.Serialize(command.DepositReturn));
}
