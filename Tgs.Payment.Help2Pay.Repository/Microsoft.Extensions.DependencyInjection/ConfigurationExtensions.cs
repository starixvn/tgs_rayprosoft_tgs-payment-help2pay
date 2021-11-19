using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tgs.Payment.Help2Pay.Configuration;
using Tgs.Payment.Help2Pay.Repository;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigurationExtensions
{
    public static Help2PayConfiguration UseRedis(this Help2PayConfiguration configuration, Action<RedisOptions, IServiceProvider> configure)
    {
        configuration.Services
            .AddOptions<RedisOptions>()
            .Configure(configure)
            .Services
            .AddSingleton<RedisConnectionFactory>();

        return configuration.UseRepository<RedisWithdrawDataStore>();
    }
}
