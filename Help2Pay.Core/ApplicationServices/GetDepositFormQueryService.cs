using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.Help2Pay.ApplicationServices;

internal class GetDepositFormQueryService : IGetService<GetDepositFormQuery, DepositFormInfo?>
{
    private readonly Help2PaySerializer _help2PaySerializer;
    private readonly IPaymentServiceClient _paymentServiceClient;

    public GetDepositFormQueryService(
        IPaymentServiceClient paymentServiceClient,
        Help2PaySerializer help2PaySerializer)
    {
        _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        _help2PaySerializer = help2PaySerializer ?? throw new ArgumentNullException(nameof(help2PaySerializer));
    }

    public async ValueTask<DepositFormInfo?> GetAsync(GetDepositFormQuery query, CancellationToken cancellationToken = default)
    {
        var formData = await _paymentServiceClient
            .GetDepositFormAsync(query.FormId, cancellationToken)
            .ConfigureAwait(false);

        if (formData == null)
            return null;

        var applyInfo = string.IsNullOrEmpty(formData.ApplyInfo)
            ? null
            : _help2PaySerializer.Deserialize<Help2PayDepositApplyInfo>(formData.ApplyInfo);

        return new DepositFormInfo(
                formData.Id,
                formData.PlayerCode,
                formData.BrandId,
                formData.BrandPaymentId,
                formData.Amount,
                formData.Status,
                applyInfo,
                formData.PaymentReturn);
    }
}
