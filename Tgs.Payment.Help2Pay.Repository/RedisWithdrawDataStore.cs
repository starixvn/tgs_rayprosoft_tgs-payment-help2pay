using StackExchange.Redis;

namespace Tgs.Payment.Help2Pay.Repository;

internal class RedisWithdrawDataStore : IWithdrawDataStore
{
    private static readonly TimeSpan _Expire = TimeSpan.FromHours(1);

    private static readonly string _KeyPrefix = "Help2Pay:WithdrawHash";

    private readonly IDatabase _redisDatabase;

    public RedisWithdrawDataStore(
        RedisConnectionFactory redisConnectionFactory)
    {
        if (redisConnectionFactory is null)
        {
            throw new ArgumentNullException(nameof(redisConnectionFactory));
        }

        _redisDatabase = redisConnectionFactory.Database;
    }

    public async ValueTask<Guid?> GetHashAsync(string hash)
    {
        var dataKey = $"{_KeyPrefix}:{hash}";

        var value = await _redisDatabase.StringGetAsync(dataKey)
            .ConfigureAwait(false);

        return value.IsNullOrEmpty ? null : Guid.Parse(value);
    }

    public async ValueTask<bool> StoreHashAsync(string hash, Guid formId)
    {
        var dataKey = $"{_KeyPrefix}:{hash}";

        return await _redisDatabase.StringSetAsync(
            dataKey,
            formId.ToString(),
            _Expire,
            when: When.NotExists).ConfigureAwait(false);
    }
}
