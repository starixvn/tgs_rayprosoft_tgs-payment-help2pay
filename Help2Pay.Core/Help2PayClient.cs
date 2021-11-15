using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.Payment.Help2Pay.Converters;

namespace Tgs.Payment.Help2Pay;

internal class Help2PayClient
{
    private readonly HttpClient _httpClient;
    private readonly Help2PayHasher _hasher;
    private readonly ILogger<Help2PayClient> _logger;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
    {
        WriteIndented = false,
        Converters =
        {
            new TwelveHourDateTimeJsonConverter(),
            new DecimalJsonConverter()
        }
    };

    public Help2PayClient(
        HttpClient httpClient,
        Help2PayHasher hasher,
        ILogger<Help2PayClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<Help2PayDepositResponse> DepositAsync(string merchantId, string securityCode, DepositOrder order, string? ip)
    {
        var info = new DepositHasherInfo(
            order.PlayerCode,
            order.Currency,
            order.Amount,
            order.TransactionId,
            order.TransactionTime);

        var hash = _hasher.ComputeHash(merchantId, securityCode, info, ip);

        var request = new DepositRequestBody
        {
            MerchantId = merchantId,
            Amount = order.Amount,
            Currency = order.Currency,
            PlayerCode = order.PlayerCode,
            ClientIp = ip,
            Hash = hash,
            TransactionId = order.TransactionId,
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

        return new Help2PayDepositResponse(html);
    }

    private IEnumerable<KeyValuePair<string, string>> SerializeToFormParameters<T>(T data)
    {
        var json = JsonSerializer.Serialize(data, _jsonSerializerOptions);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
    }
}