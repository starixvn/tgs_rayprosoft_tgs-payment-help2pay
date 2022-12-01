using System.Reflection;
using Microsoft.Extensions.Options;
using Tgs.Payment.Help2Pay;
using Tgs.Payment.Help2Pay.Configuration;
using Tgs.Payment.Help2Pay.MessageQueue;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static Help2PayConfiguration AddHelp2Pay(this IServiceCollection services, Action<Help2PayOptions, IServiceProvider> configure)
    {
        _ = services
            .AddOptions<Help2PayOptions>()
            .Configure(configure)
            .Services
            .RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
            .AddTransient<Help2PayHasher>()
            .AddTransient<Help2PaySerializer>()
            .AddMessageQueue()
            .AddNatsMessageQueue(config => config
                .ConfigQueueOptions((options, sp) =>
                {
                    var help2PayOptions = sp.GetRequiredService<IOptions<Help2PayOptions>>().Value;
                    options.Url = help2PayOptions.NatsUrl;
                })
                .AddProcessor<DepositProcessor>("tgs.payment.help2pay.deposit", "tgs.payment.help2pay.deposit")
                .AddProcessor<WithdrawProcessor>("tgs.payment.help2pay.withdraw", "tgs.payment.help2pay.withdraw"))
            .AddNatsGlobPatternExchange("*");

        _ = services
            .AddHttpClient<Help2PayDepositClient>((sp, httpClient)
            =>
            {
                var options = sp.GetRequiredService<IOptions<Help2PayOptions>>().Value;
                httpClient.BaseAddress = options.PaymentDepositApiUrl;
            });

        _ = services
            .AddHttpClient<Help2PayQueryClient>((sp, httpClient)
            =>
            {
                var options = sp.GetRequiredService<IOptions<Help2PayOptions>>().Value;
                httpClient.BaseAddress = options.PaymentQueryApiUrl;
            });

        _ = services
            .AddHttpClient<Help2PayWithdrawClient>((sp, httpClient)
            =>
            {
                var options = sp.GetRequiredService<IOptions<Help2PayOptions>>().Value;
                httpClient.BaseAddress = options.PaymentWithdrawApiUrl;
            });

        _ = services
            .AddPaymentServiceClient(httpClientBuilder => httpClientBuilder
                .ConfigureHttpClient((sp, httpClient) =>
            {
                var help2PayOptions = sp.GetRequiredService<IOptions<Help2PayOptions>>().Value;
                httpClient.BaseAddress = help2PayOptions.PaymentServiceBaseUrl;
            }))
            .AddPaymentClient();

        var configuration = new Help2PayConfiguration(services);

        return configuration;
    }
}
