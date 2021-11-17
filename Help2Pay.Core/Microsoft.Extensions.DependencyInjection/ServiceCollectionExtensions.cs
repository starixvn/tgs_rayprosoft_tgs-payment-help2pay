using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        services
            .AddHttpClient<Help2PayDepositClient>()
            .ConfigureHttpClient((sp, httpClient)
            =>
            {
                var options = sp.GetRequiredService<IOptions<Help2PayOptions>>().Value;
                httpClient.BaseAddress = options.PaymentDepositApiUrl;
            });

        services
            .AddHttpClient<Help2PayQueryClient>()
            .ConfigureHttpClient((sp, httpClient)
            =>
            {
                var options = sp.GetRequiredService<IOptions<Help2PayOptions>>().Value;
                httpClient.BaseAddress = options.PaymentQueryApiUrl;
            });

        services
            .AddPaymentServiceClient((options, sp) =>
            {
                var help2PayOptions = sp.GetRequiredService<IOptions<Help2PayOptions>>().Value;
                options.PaymentServiceBaseUrl = help2PayOptions.PaymentServiceBaseUrl;

                options.JsonSerializerOptions = options.JsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters =
                        {
                            new JsonStringEnumConverter()
                        }
                };
            })
            .AddPaymentClient();

        return services;
    }
}