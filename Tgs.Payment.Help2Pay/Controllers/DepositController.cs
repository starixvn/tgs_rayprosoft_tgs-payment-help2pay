using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.Payment.Clients;
using Tgs.Payment.Help2Pay.Services;
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
        [FromForm] DepositConfirmViewModel depositModel)
    {
        var jsonSerializerOptions = options.Value.SystemSerializerOptions;
        var data = JsonSerializer.Serialize(depositModel, jsonSerializerOptions);
        _logger.LogInformation($"deposit callback: {data}");

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

            if (depositModel.Status == "000")
            {
                var approveCommand = new ApproveDepositFormCommand(
                            depositForm.Id,
                            depositForm.BrandId,
                            depositModel.Amount);

                await approveService.ExecuteAsync(approveCommand).ConfigureAwait(false);
            }

            return Ok("success");
        }

        throw new Exception();
    }
}
