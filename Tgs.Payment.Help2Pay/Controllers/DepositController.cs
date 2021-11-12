using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tgs.Payment.Help2Pay.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DepositController : ControllerBase
{
    private readonly ILogger<DepositController> _logger;

    public DepositController(
        ILogger<DepositController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}