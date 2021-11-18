using Microsoft.Extensions.Logging;

namespace Tgs.Payment.Help2Pay;

internal class Help2PayWithdrawClient
{
    private readonly ILogger<Help2PayWithdrawClient> _logger;

    public Help2PayWithdrawClient(
        ILogger<Help2PayWithdrawClient> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ValueTask<CreateWithdrawFormResponse> WithdrawAsync(string merchantId, string securityCode, WithdrawOrder order)
    {
        return default;
    }
}
