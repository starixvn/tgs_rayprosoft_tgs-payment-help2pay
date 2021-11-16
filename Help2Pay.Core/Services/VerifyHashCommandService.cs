using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.Payment.Help2Pay.Services;

internal class VerifyHashCommandService : IGetService<VerifyHashCommand, bool>
{
    private readonly ILogger<VerifyHashCommandService> _logger;
    private readonly Help2PayHasher _help2PayHasher;

    public VerifyHashCommandService(
        ILogger<VerifyHashCommandService> logger,
        Help2PayHasher help2PayHasher)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _help2PayHasher = help2PayHasher ?? throw new ArgumentNullException(nameof(help2PayHasher));
    }

    public ValueTask<bool> GetAsync(VerifyHashCommand command, CancellationToken cancellationToken = default)
    {
        var info = new DepositConfirmHasherInfo(
            command.PlayerCode,
            command.Currency,
            command.Amount,
            command.ReferenceId,
            command.StatusCode);

        var hash = _help2PayHasher.ComputeHash(command.MerchantId, command.SecurityCode, info, null);

        _logger.LogInformation($"Callback hash key: {command.HashKey}, data hashed: {hash}");

        var result = command.HashKey.Equals(hash, StringComparison.OrdinalIgnoreCase);

        return ValueTask.FromResult(result);
    }
}