using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.Payment.Clients;
using Tgs.Payment.Help2Pay.ViewModels;
using Tgs.ServiceUtility;
using Tgs.Payment.Help2Pay.ApplicationServices;

namespace Tgs.Payment.Help2Pay.Controllers;

[Route("[controller]")]
[ApiController]
public class WithdrawController : ControllerBase
{
    private readonly ILogger<WithdrawController> _logger;

    public WithdrawController(
        ILogger<WithdrawController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("validate")]
    public async ValueTask<string> ValidateWithdrawFormAsync(
        [FromQuery(Name = "transId")] Guid formId,
        [FromQuery(Name = "key")] string? hash,
        [FromServices] IGetService<GetWithdrawFormQuery, WithdrawFormInfo?> getWithdrawFormService,
        [FromServices] IWithdrawDataStore store)
    {
        _logger.LogInformation($"Withdraw validation input: transId='{formId}', hash={hash}");

        if (string.IsNullOrEmpty(hash))
        {
            _logger.LogError($"Withdraw form id='{formId}', input hash is null or empty");
            return "false";
        }

        var storedFormId = await store.GetHashAsync(hash!).ConfigureAwait(false);

        if (storedFormId == null || storedFormId.Value != formId)
        {
            _logger.LogError($"Withdraw validataion input form id='{formId}' is not matched as expected form id='{storedFormId}'");
            return "false";
        }

        var info = await getWithdrawFormService
            .GetAsync(new GetWithdrawFormQuery(formId))
            .ConfigureAwait(false);

        if (info!.Status == PaymentFlowState.Paid)
        {
            _logger.LogError($"Withdraw form with id='{formId}' is paid already");
            return "false";
        }

        return "true";
    }

    [HttpPost("callback")]
    public async ValueTask<IActionResult> WithdrawCallbackAsync(
        [FromServices] IOptions<Help2PayOptions> options,
        [FromServices] IGetService<GetWithdrawFormQuery, WithdrawFormInfo?> getWithdrawFormService,
        [FromServices] IGetService<VerifyWithdrawConfirmHashCommand, bool> verifyHashCommandService,
        [FromServices] ICommandService<SaveWithdrawReturnCommand> saveReturnService,
        [FromForm] IFormCollection formData)
    {
        var jsonSerializerOptions = options.Value.SystemSerializerOptions;

        var dict = formData.ToDictionary(x => x.Key, x => x.Value.ToString());
        var data = JsonSerializer.Serialize(dict, jsonSerializerOptions);

        _logger.LogInformation($"Withdraw callback: {data}");

        var withdrawModel = JsonSerializer.Deserialize<WithdrawConfirmViewModel>(data, jsonSerializerOptions)!;

        var withdrawForm = await getWithdrawFormService
               .GetAsync(new GetWithdrawFormQuery(withdrawModel.TransactionId))
               .ConfigureAwait(false);

        var verifyHashCommand = new VerifyWithdrawConfirmHashCommand(
                withdrawModel.MerchantId,
                withdrawForm!.ApplyInfo!.Settings.SecurityCode,
                withdrawModel.PlayerCode,
                withdrawModel.Currency,
                withdrawModel.Amount,
                withdrawModel.TransactionId,
                withdrawModel.Status,
                withdrawModel.Hash);

        var verified = await verifyHashCommandService
            .GetAsync(verifyHashCommand)
            .ConfigureAwait(false);

        if (!verified)
        {
            throw new Exception("Invalid hash key");
        }

        var returnData = new Help2PayWithdrawReturn(
            withdrawModel.MerchantId,
            withdrawModel.WithdrawId,
            withdrawModel.PlayerCode,
            withdrawModel.Currency,
            withdrawModel.TransactionId,
            withdrawModel.Amount,
            withdrawModel.PayoutFee,
            withdrawModel.TransactionTime,
            withdrawModel.Status,
            withdrawModel.Message);

        if (withdrawModel.Status == "000")
        {
            var formStatus = withdrawForm.Status == PaymentFlowState.Approved
                ? PaymentFlowState.Paid
                : withdrawForm.Status;

            await saveReturnService
                    .ExecuteAsync(new SaveWithdrawReturnCommand(
                        withdrawForm.Id,
                        formStatus,
                        returnData.Amount,
                        returnData))
                    .ConfigureAwait(false);
        }
        else
        {
            await saveReturnService
                    .ExecuteAsync(new SaveWithdrawReturnCommand(
                        withdrawForm.Id,
                        withdrawForm.Status,
                        null,
                        returnData))
                    .ConfigureAwait(false);
        }

        return Ok("success");
    }
}
