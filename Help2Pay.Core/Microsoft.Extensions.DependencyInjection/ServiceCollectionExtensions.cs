using System.Reflection;
using Microsoft.Extensions.Options;
using Tgs.Payment.Help2Pay;
using Tgs.Payment.Help2Pay.MessageQueue;

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
            .AddTransient<Help2PayHasher>()
            .AddTransient<Help2PaySerializer>()
            .AddMessageQueue()
            .AddNatsMessageQueue(config =>
            {
                config
                    .ConfigQueueOptions((options, sp) =>
                    {
                        var help2PayOptions = sp.GetRequiredService<IOptions<Help2PayOptions>>().Value;
                        options.Url = help2PayOptions.NatsUrl;
                    })
                    .AddProcessor<DepositProcessor>("tgs.payment.help2pay.deposit", "tgs.payment.help2pay.deposit");
            })
            .AddNatsGlobPatternExchange("*");

        return services;
    }
}