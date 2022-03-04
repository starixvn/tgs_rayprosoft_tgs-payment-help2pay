using System.Xml.Serialization;
using Microsoft.Extensions.Logging;

namespace Tgs.Payment.Help2Pay;

internal class Help2PayQueryClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<Help2PayQueryClient> _logger;

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

        _logger.LogInformation($"Query Payment Deposit Id Body: {data}");

        _ = response.EnsureSuccessStatusCode();

        using var sr = new StringReader(data);
        var serializer = new XmlSerializer(typeof(QueryPaymentDespositFormResponse));
        var depositResponse = (QueryPaymentDespositFormResponse)serializer.Deserialize(sr)!;

        return string.IsNullOrWhiteSpace(depositResponse.DepositId) || depositResponse.DepositId == "0"
            ? throw new ApiException($"Invalid Payment Deposit Id.")
            : depositResponse.DepositId;
    }

    public async ValueTask<string> QueryPaymentWithdrawIdAsync(string merchantId, Guid formId)
    {
        var response = await _httpClient
            .PostAsync($"/Services/Merchants/{merchantId}/FundOutStatus/{formId}", null)
            .ConfigureAwait(false);

        _logger.LogInformation($"Query Payment Withdraw Id StatusCode: {response.StatusCode}");

        var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        _logger.LogInformation($"Query Payment Withdraw Id Body: {data}");

        _ = response.EnsureSuccessStatusCode();

        using var sr = new StringReader(data);
        var serializer = new XmlSerializer(typeof(QueryPaymentWithdrawFormResponse));
        var withdrawResponse = (QueryPaymentWithdrawFormResponse)serializer.Deserialize(sr)!;

        return string.IsNullOrWhiteSpace(withdrawResponse.WithdrawId) || withdrawResponse.WithdrawId == "0"
            ? throw new ApiException($"Invalid Payment Withdraw Id.")
            : withdrawResponse.WithdrawId;
    }
}
