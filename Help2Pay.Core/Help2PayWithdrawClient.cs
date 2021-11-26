using System.Text.Json;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using Tgs.Payment.Help2Pay.Converters;

namespace Tgs.Payment.Help2Pay;

internal class Help2PayWithdrawClient
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

    private readonly ILogger<Help2PayWithdrawClient> _logger;
    private readonly IWithdrawDataStore _store;

    public Help2PayWithdrawClient(
        HttpClient httpClient,
        Help2PayHasher hasher,
        IWithdrawDataStore store,
        ILogger<Help2PayWithdrawClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<WithdrawFormResponse> WithdrawAsync(string merchantId, string securityCode, WithdrawOrder order)
    {
        var info = new WithdrawHasherInfo
        {
            MemberCode = order.MemberCode,
            CurrencyCode = order.Currency,
            Amount = order.Amount,
            TransactionId = order.TransactionId,
            TransactionTime = order.TransactionTime,
            BankAccount = order.BankAccount
        };

        var hash = _hasher.ComputeHash(merchantId, securityCode, info);

        await _store.StoreHashAsync(hash, order.TransactionId).ConfigureAwait(false);

        var request = new WithdrawRequestBody
        {
            MerchantId = merchantId,
            Amount = order.Amount,
            BankAccount = order.BankAccount,
            BankAccountName = order.BankAccountName,
            BankCode = order.BankCode,
            Currency = order.Currency,
            TransactionId = order.TransactionId,
            MemberCode = order.MemberCode,
            TransactionTime = order.TransactionTime,
            ClientIp = order.ClientIp,
            CallbackUrl = order.CallbackUrl,
            Hash = hash
        };

        var parameters = SerializeToFormParameters(request);
        var requestContent = new FormUrlEncodedContent(parameters);
        var response = await _httpClient
            .PostAsync($"/MerchantPayout/{merchantId}", requestContent)
            .ConfigureAwait(false);

        _logger.LogInformation($"Withdraw StatusCode: {response.StatusCode}");

        var xmlBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        _logger.LogInformation($"Withdraw Body: {xmlBody}");

        response.EnsureSuccessStatusCode();

        using var sr = new StringReader(xmlBody);
        var serializer = new XmlSerializer(typeof(WithdrawFormResponse));
        return (WithdrawFormResponse)serializer.Deserialize(sr)!;
    }

    private IEnumerable<KeyValuePair<string, string>> SerializeToFormParameters<T>(T data)
    {
        var json = JsonSerializer.Serialize(data, _jsonSerializerOptions);

        _logger.LogInformation($"Withdraw output parameters: {json}");

        return JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
    }
}
