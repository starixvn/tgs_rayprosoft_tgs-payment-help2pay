using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Tgs.Payment.Help2Pay.Repository;

internal class RedisConnectionFactory : IDisposable
{
    private readonly ConnectionMultiplexer _connectionMultiplexer;
    private bool _disposedValue;

    public IDatabase Database { get; }

    public RedisConnectionFactory(IOptions<RedisOptions> optionsAccessor)
    {
        var options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));

        var connectionOptions = ConfigurationOptions.Parse(options.ConnectionString);

        _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionOptions);

        Database = _connectionMultiplexer.GetDatabase();
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _connectionMultiplexer?.Dispose();
            }

            _disposedValue = true;
        }
    }
}
