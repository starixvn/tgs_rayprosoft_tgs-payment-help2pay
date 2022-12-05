using System.Reflection;
using Microsoft.Extensions.Options;
using Tgs.Payment.Help2Pay;
using Tgs.Payment.Help2Pay.Configuration;
using Tgs.Payment.Help2Pay.MessageQueue;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static Help2PayConfiguration AddHelp2Pay(
        this IServiceCollection services,
        Action<Help2PayOptions, IServiceProvider> configure,
        Func<IServiceProvider, DelegatingHandler> httpClientMessageHandlerFactory)
    {
        _ = services
            .AddOptions<Help2PayOptions>()
            .Configure(configure)
            .Services
            .RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient)
            .AddTransient<Help2PayHasher>()
            .AddTransient<Help2PaySerializer>();

        _ = services
            .AddHttpClient<Help2PayDepositClient>()
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<Help2PayOptions>>().Value;
                httpClient.BaseAddress = options.PaymentDepositApiUrl;
            })
            .AddHttpMessageHandler(httpClientMessageHandlerFactory);

        _ = services
            .AddHttpClient<Help2PayQueryClient>()
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<Help2PayOptions>>().Value;
                httpClient.BaseAddress = options.PaymentQueryApiUrl;
            })
            .AddHttpMessageHandler(httpClientMessageHandlerFactory);

        _ = services
            .AddHttpClient<Help2PayWithdrawClient>()
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<Help2PayOptions>>().Value;
                httpClient.BaseAddress = options.PaymentWithdrawApiUrl;
            })
            .AddHttpMessageHandler(httpClientMessageHandlerFactory);

        _ = services
            .AddPaymentServiceClient(httpClientBuilder => httpClientBuilder
                .ConfigureHttpClient((sp, httpClient) =>
                {
                    var help2PayOptions = sp.GetRequiredService<IOptions<Help2PayOptions>>().Value;
                    httpClient.BaseAddress = help2PayOptions.PaymentServiceBaseUrl;
                })
                .AddHttpMessageHandler(httpClientMessageHandlerFactory))
            .AddPaymentClient();

        var configuration = new Help2PayConfiguration(services);

        return configuration;
    }
}
