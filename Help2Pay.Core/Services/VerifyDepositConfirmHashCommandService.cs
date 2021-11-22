using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.Payment.Help2Pay.Services;

internal class VerifyDepositConfirmHashCommandService : IGetService<VerifyDepositConfirmHashCommand, bool>
{
    private readonly Help2PayHasher _help2PayHasher;
    private readonly ILogger<VerifyDepositConfirmHashCommandService> _logger;

    public VerifyDepositConfirmHashCommandService(
        ILogger<VerifyDepositConfirmHashCommandService> logger,
        Help2PayHasher help2PayHasher)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _help2PayHasher = help2PayHasher ?? throw new ArgumentNullException(nameof(help2PayHasher));
    }

    public ValueTask<bool> GetAsync(VerifyDepositConfirmHashCommand command, CancellationToken cancellationToken = default)
    {
        var info = new DepositConfirmHasherInfo(
            command.PlayerCode,
            command.Currency,
            command.Amount,
            command.ReferenceId,
            command.StatusCode);

        var hash = _help2PayHasher.ComputeHash(command.MerchantId, command.SecurityCode, info);

        _logger.LogInformation($"Deposit callback hash key: {command.HashKey}, data hashed: {hash}");

        var result = command.HashKey.Equals(hash, StringComparison.OrdinalIgnoreCase);

        return ValueTask.FromResult(result);
    }
}
