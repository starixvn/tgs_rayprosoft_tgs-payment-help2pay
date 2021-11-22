using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.Help2Pay.ApplicationServices;

internal class GetWithdrawFormQueryService : IGetService<GetWithdrawFormQuery, WithdrawFormInfo?>
{
    private readonly Help2PaySerializer _help2PaySerializer;
    private readonly IPaymentServiceClient _paymentServiceClient;

    public GetWithdrawFormQueryService(
        IPaymentServiceClient paymentServiceClient,
        Help2PaySerializer help2PaySerializer)
    {
        _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        _help2PaySerializer = help2PaySerializer ?? throw new ArgumentNullException(nameof(help2PaySerializer));
    }

    public async ValueTask<WithdrawFormInfo?> GetAsync(
        GetWithdrawFormQuery query,
        CancellationToken cancellationToken = default)
    {
        var data = await _paymentServiceClient
            .GetWithdrawFormAsync(query.FormId)
            .ConfigureAwait(false);

        return data == null ?
            null :
            new WithdrawFormInfo(
                data.Id,
                data.UserId,
                data.UserType,
                data.BrandId,
                data.BrandPaymentId,
                data.Amount,
                data.Status,
                data.BankInfoId,
                data.BankInfo,
                string.IsNullOrWhiteSpace(data.ApplyInfo)
                    ? null
                    : _help2PaySerializer.Deserialize<Help2PayWithdrawApplyInfo>(data.ApplyInfo));
    }
}
