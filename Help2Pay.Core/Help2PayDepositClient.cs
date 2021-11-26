using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.Payment.Help2Pay.Converters;

namespace Tgs.Payment.Help2Pay;

internal class Help2PayDepositClient
{
    private readonly Help2PayHasher _hasher;
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        WriteIndented = false,
        Converters =
        {
            new TwelveHourDateTimeJsonConverter(),
            new DecimalJsonConverter()
        }
    };

    private readonly ILogger<Help2PayDepositClient> _logger;

    public Help2PayDepositClient(
        HttpClient httpClient,
        Help2PayHasher hasher,
        ILogger<Help2PayDepositClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<string> DepositAsync(string merchantId, string securityCode, DepositOrder order)
    {
        var info = new DepositHasherInfo(
            order.Customer,
            order.Currency,
            order.Amount,
            order.TransactionId,
            order.TransactionTime);

        var hash = _hasher.ComputeHash(
            merchantId,
            securityCode,
            info,
            order.ClientIp);

        var request = new DepositRequestBody
        {
            MerchantId = merchantId,
            Amount = order.Amount,
            Currency = order.Currency,
            Customer = order.TransactionId.ToString(),
            ClientIp = order.ClientIp,
            Hash = hash,
            ReferenceId = order.TransactionId,
            TransactionTime = order.TransactionTime,
            BankCode = order.BankCode,
            CallbackUrl = order.CallbackUrl,
            ResponseUrl = order.ResponseUrl,
            Language = "en-us"
        };

        var parameters = SerializeToFormParameters(request);
        var requestContent = new FormUrlEncodedContent(parameters);
        var response = await _httpClient
            .PostAsync("/MerchantTransfer", requestContent)
            .ConfigureAwait(false);

        _logger.LogInformation($"Deposit StatusCode: {response.StatusCode}");

        var html = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        _logger.LogInformation($"Deposit Body: {html}");

        response.EnsureSuccessStatusCode();

        if (string.IsNullOrWhiteSpace(html))
        {
            throw new ApiException("Responsed html should not be null or empty.");
        }

        return html;
    }

    private IEnumerable<KeyValuePair<string, string>> SerializeToFormParameters<T>(T data)
    {
        var json = JsonSerializer.Serialize(data, _jsonSerializerOptions);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
    }
}
