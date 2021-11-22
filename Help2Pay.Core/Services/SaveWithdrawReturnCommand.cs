using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.Payment.Clients;

namespace Tgs.Payment.Help2Pay.Services;

public record SaveWithdrawReturnCommand(
    Guid FormId,
        PaymentFlowState State,
        decimal? ActualAmount,
    Help2PayWithdrawReturn WithdrawReturn);
