using System.Reflection;
using Microsoft.Extensions.Options;
using Tgs.Payment.Help2Pay;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHelp2Pay(this IServiceCollection services, Action<Help2PayOptions, IServiceProvider> configure)
    {
        services
            .AddOptions<Help2PayOptions>()
            .Configure(configure)
            .Services
            .RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
            .AddSingleton<Help2PayHasher>()
            .AddMessageQueue()
            .AddNatsMessageQueue(config =>
            {
                config.ConfigQueueOptions((options, sp) =>
                {
                    var help2PayOptions = sp.GetRequiredService<IOptions<Help2PayOptions>>().Value;
                    options.Url = help2PayOptions.NatsUrl;
                });
            })
            .AddNatsGlobPatternExchange("*");

        return services;
    }
}