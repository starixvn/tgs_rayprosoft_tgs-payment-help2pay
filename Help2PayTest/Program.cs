// See https://aka.ms/new-console-template for more information
using Help2PayTest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tgs.Payment.Help2Pay;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args)
    => Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            _ = services.AddMessageQueue()
                .AddNatsMessageQueue(_ => { })
                .AddNatsGlobPatternExchange("*");

            _ = services
                .AddPaymentClient()
                .AddSingleton<Help2PaySerializer>()
                .AddHostedService<Demo>();
        });
