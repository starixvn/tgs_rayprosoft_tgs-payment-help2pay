using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tgs.Payment.Help2Pay;

internal class Help2PayClient
{
    private readonly HttpClient _client;
    private readonly Help2PayHasher _hasher;
    private readonly ILogger<Help2PayClient> _logger;

    public Help2PayClient(
        HttpClient client,
        Help2PayHasher hasher,
        ILogger<Help2PayClient> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<string?> DepositAsync(string securityCode, DepositOrder order)
    {
        return default;
    }
}