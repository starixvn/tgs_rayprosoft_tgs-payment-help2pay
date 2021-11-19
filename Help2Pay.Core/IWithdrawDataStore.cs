namespace Tgs.Payment.Help2Pay;

public interface IWithdrawDataStore
{
    ValueTask<string?> GetHashAsync(Guid formId);

    ValueTask<bool> StoreHashAsync(Guid formId, string hash);
}
