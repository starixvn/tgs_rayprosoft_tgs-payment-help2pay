using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.Payment.Clients;
using Tgs.Payment.Help2Pay.Services;
using Tgs.Payment.Help2Pay.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.Payment.Help2Pay.Controllers;

[Route("[controller]")]
[ApiController]
public class WithdrawController : ControllerBase
{
    private readonly ILogger<WithdrawController> _logger;
    private readonly IWithdrawDataStore _store;

    public WithdrawController(
        ILogger<WithdrawController> logger,
        IWithdrawDataStore store
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    [HttpPost("validate")]
    public async ValueTask<string> ValidateWithdrawFormAsync(
        [FromQuery(Name = "transId")] Guid formId,
        [FromQuery(Name = "key")] string? hash,
        [FromServices] IGetService<GetWithdrawFormQuery, WithdrawFormInfo?> getWithdrawFormService)
    {
        _logger.LogInformation($"transId='{formId}', hash={hash}");

        var storedHash = await _store.GetHashAsync(formId).ConfigureAwait(false);

        _logger.LogInformation($"withdraw form id='{formId}', storedHash={storedHash}");

        if (string.IsNullOrEmpty(storedHash))
        {
            _logger.LogError($"withdraw form id='{formId}', stored hash is null or empty");
            return "false";
        }

        if (!storedHash.Equals(hash, StringComparison.OrdinalIgnoreCase))
        {
            return "false";
        }

        var info = await getWithdrawFormService
            .GetAsync(new GetWithdrawFormQuery(formId))
            .ConfigureAwait(false);

        if (info!.Status != PaymentFlowState.Approved)
        {
            _logger.LogError($"withdraw form with id='{formId}' is not at Approved status. status={(int)info.Status}");
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
        [FromForm] WithdrawConfirmViewModel withdrawModel)
    {
        var jsonSerializerOptions = options.Value.SystemSerializerOptions;
        var data = JsonSerializer.Serialize(withdrawModel, jsonSerializerOptions);
        _logger.LogInformation($"withdraw callback: {data}");

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
