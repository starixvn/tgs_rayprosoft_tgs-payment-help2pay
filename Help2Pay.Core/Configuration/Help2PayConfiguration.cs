using Microsoft.Extensions.DependencyInjection;

namespace Tgs.Payment.Help2Pay.Configuration;

public class Help2PayConfiguration
{
    public IServiceCollection Services { get; }

    public Help2PayConfiguration(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public Help2PayConfiguration UseRepository<TRepository>() where TRepository : class, IWithdrawDataStore
    {
        _ = Services.AddTransient<IWithdrawDataStore, TRepository>();

        return this;
    }
}
