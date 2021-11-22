using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.Payment.Help2Pay.Services;

internal class VerifyWithdrawConfirmHashCommandService : IGetService<VerifyWithdrawConfirmHashCommand, bool>
{
    private readonly Help2PayHasher _help2PayHasher;
    private readonly ILogger<VerifyWithdrawConfirmHashCommandService> _logger;

    public VerifyWithdrawConfirmHashCommandService(
            ILogger<VerifyWithdrawConfirmHashCommandService> logger,
            Help2PayHasher help2PayHasher)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _help2PayHasher = help2PayHasher ?? throw new ArgumentNullException(nameof(help2PayHasher));
    }

    public ValueTask<bool> GetAsync(
        VerifyWithdrawConfirmHashCommand command,
        CancellationToken cancellationToken = default)
    {
        var info = new WithdrawConfirmHasherInfo(
            command.PlayerCode,
            command.Currency,
            command.Amount,
            command.TransactionId,
            command.StatusCode);

        var hash = _help2PayHasher.ComputeHash(command.MerchantId, command.SecurityCode, info);

        _logger.LogInformation($"Withdraw callback hash key: {command.HashKey}, data hashed: {hash}");

        var result = command.HashKey.Equals(hash, StringComparison.OrdinalIgnoreCase);

        return ValueTask.FromResult(result);
    }
}
