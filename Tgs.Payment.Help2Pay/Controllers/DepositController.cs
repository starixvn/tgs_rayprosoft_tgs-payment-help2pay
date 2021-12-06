using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.Payment.Clients;
using Tgs.Payment.Help2Pay.ApplicationServices;
using Tgs.Payment.Help2Pay.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.Payment.Help2Pay.Controllers;

[Route("[controller]")]
[ApiController]
public class DepositController : ControllerBase
{
    private readonly ILogger<DepositController> _logger;

    public DepositController(
        ILogger<DepositController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("callback")]
    public async Task<IActionResult> DepositCallbackAsync(
        [FromServices] IOptions<Help2PayOptions> options,
        [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo?> getDepositFormService,
        [FromServices] IGetService<VerifyDepositConfirmHashCommand, bool> verifyHashCommandService,
        [FromServices] ICommandService<SaveDepositReturnCommand> saveReturnService,
        [FromServices] ICommandService<ApproveDepositFormCommand> approveService,
        [FromForm] IFormCollection formData)
    {
        var jsonSerializerOptions = options.Value.SystemSerializerOptions;

        var dict = formData.ToDictionary(x => x.Key, x => x.Value.ToString());
        var data = JsonSerializer.Serialize(dict, jsonSerializerOptions);

        _logger.LogInformation($"deposit callback: {data}");

        var depositModel = JsonSerializer.Deserialize<DepositConfirmViewModel>(data, jsonSerializerOptions)!;

        var depositForm = await getDepositFormService
            .GetAsync(new GetDepositFormQuery(depositModel.ReferenceId))
            .ConfigureAwait(false);

        var verifyHashCommand = new VerifyDepositConfirmHashCommand(
                depositModel.MerchantId,
                depositForm!.ApplyInfo!.Settings.SecurityCode,
                depositModel.PlayerCode,
                depositModel.Currency,
                depositModel.Amount,
                depositModel.ReferenceId,
                depositModel.Status,
                depositModel.Hash);

        var verified = await verifyHashCommandService
            .GetAsync(verifyHashCommand)
            .ConfigureAwait(false);

        if (!verified)
        {
            throw new Exception("Invalid hash key");
        }

        if (depositForm.Status < PaymentFlowState.WaitApprove)
        {
            var paymentReturn = new Help2PayDepositReturn(
                    depositModel.MerchantId,
                    depositModel.DepositId,
                    depositModel.PlayerCode,
                    depositModel.Currency,
                    depositModel.ReferenceId,
                    depositModel.Amount,
                    depositModel.DepositFee,
                    depositModel.TransactionTime,
                    depositModel.StatementTime,
                    depositModel.Status,
                    depositModel.ErrorCode,
                    depositModel.Note);

            var saveReturnCommand = new SaveDepositReturnCommand(depositForm.Id, depositForm.Status, paymentReturn);

            await saveReturnService.ExecuteAsync(saveReturnCommand).ConfigureAwait(false);

            // Deposit callback status:
            // 000 => Success  ( System process with no error )
            // 006 => Approved ( Manual Approved )
            if (depositModel.Status is "000" or "006")
            {
                var approveCommand = new ApproveDepositFormCommand(
                            depositForm.Id,
                            depositForm.BrandId,
                            depositModel.Amount / depositForm.ApplyInfo.Settings.ExchangeRate);

                await approveService.ExecuteAsync(approveCommand).ConfigureAwait(false);
            }

            return Ok("success");
        }

        throw new Exception();
    }
}
