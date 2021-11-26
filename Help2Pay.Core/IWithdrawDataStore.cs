namespace Tgs.Payment.Help2Pay;

public interface IWithdrawDataStore
{
    ValueTask<Guid?> GetHashAsync(string hash);

    ValueTask<bool> StoreHashAsync(string hash, Guid formId);
}
