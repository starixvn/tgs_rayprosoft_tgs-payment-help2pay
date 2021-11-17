using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;

namespace Tgs.Payment.Help2Pay;

internal class Help2PayQueryClient
{
    private readonly ILogger<Help2PayQueryClient> _logger;
    private readonly HttpClient _httpClient;

    public Help2PayQueryClient(
        ILogger<Help2PayQueryClient> logger,
        HttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async ValueTask<string> QueryPaymentDepositIdAsync(string merchantId, Guid formId)
    {
        var response = await _httpClient
            .PostAsync($"/Services/Merchants/{merchantId}/TransferStatus/{formId}", null)
            .ConfigureAwait(false);

        _logger.LogInformation($"Query Payment Deposit Id StatusCode: {response.StatusCode}");

        var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        _logger.LogInformation($"Query Payment Deposit Id  Body: {data}");

        response.EnsureSuccessStatusCode();

        using var sr = new StringReader(data);
        var serializer = new XmlSerializer(typeof(QueryPaymentDespositFormResponse));
        var depositResponse = (QueryPaymentDespositFormResponse)serializer.Deserialize(sr)!;

        if (!string.IsNullOrWhiteSpace(depositResponse.DepositId))
        {
            return depositResponse.DepositId;
        }

        throw new ApiException($"Payment Deposit Id should not be null or empty: {data}");
    }
}