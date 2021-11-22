using StackExchange.Redis;

namespace Tgs.Payment.Help2Pay.Repository;

internal class RedisWithdrawDataStore : IWithdrawDataStore
{
    private static readonly TimeSpan _Expire = TimeSpan.FromHours(1);

    private static readonly string _KeyPrefix = "Help2Pay:WithdrawHash";

    private IDatabase _redisDatabase;

    public RedisWithdrawDataStore(
        RedisConnectionFactory redisConnectionFactory)
    {
        if (redisConnectionFactory is null)
        {
            throw new ArgumentNullException(nameof(redisConnectionFactory));
        }

        _redisDatabase = redisConnectionFactory.Database;
    }

    public async ValueTask<string?> GetHashAsync(Guid formId)
    {
        var dataKey = $"{_KeyPrefix}:{formId}";

        var value = await _redisDatabase.StringGetAsync(dataKey)
            .ConfigureAwait(false);

        return value;
    }

    public async ValueTask<bool> StoreHashAsync(Guid formId, string hash)
    {
        var dataKey = $"{_KeyPrefix}:{formId}";

        return await _redisDatabase.StringSetAsync(dataKey, hash, _Expire, When.NotExists)
            .ConfigureAwait(false);
    }
}
