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
            services.AddMessageQueue()
                .AddNatsMessageQueue(_ => { })
                .AddNatsGlobPatternExchange("*");

            services.AddPaymentClient();
            services.AddSingleton<Help2PaySerializer>();

            services.AddHostedService<Demo>();
        });