using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.Payment.Help2Pay.Services;
using Tgs.ServiceUtility;

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
    public ValueTask<string> ValidateWithdrawFormAsync(
        [FromQuery(Name = "transId")] Guid formId,
        [FromQuery(Name = "key")] string? hash,
        [FromServices] IGetService<GetWithdrawFormQuery, WithdrawFormInfo?> getWithdrawFormService)
    {
        _logger.LogInformation($"transId={formId}, hash={hash}");

        return new ValueTask<string>(bool.TrueString);
    }

    [HttpPost("callback")]
    public async ValueTask<IActionResult> WithdrawCallbackFormAsync(
        [FromServices] IOptions<Help2PayOptions> options)
    {
        var data = Request.Form.ToDictionary(p => p.Key, p => p.Value.ToString());

        var value = JsonSerializer.Serialize(data, options?.Value.SystemSerializerOptions);

        _logger.LogInformation($"withdraw callback: {value}");

        return Ok("success");
    }
}
