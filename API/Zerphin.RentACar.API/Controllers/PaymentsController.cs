using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zerphin.RentACar.Application.Features.Payments.CreatePayment;
using Zerphin.RentACar.Application.Features.Payments.DeletePayment;
using Zerphin.RentACar.Application.Features.Payments.GetPaymentById;
using Zerphin.RentACar.Application.Features.Payments.SearchPayments;
using Zerphin.RentACar.Application.Features.Payments.UpdatePayment;
using Zerphin.RentACar.Application.Features.Payments.GetPaymentStatistics;
using Zerphin.RentACar.Domain.Attributes;

namespace Zerphin.RentACar.API.Controllers;

public class PaymentsController : BaseController
{
    public PaymentsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [RequireRole("admin", "manager", "employee")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("id/")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> GetPaymentById([FromQuery] GetPaymentByIdCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("search")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> SearchPayments([FromQuery] SearchPaymentsCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpPut]
    [RequireRole("admin", "manager", "employee")]
    public async Task<IActionResult> UpdatePayment([FromBody] UpdatePaymentCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpDelete]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> DeletePayment([FromQuery] DeletePaymentCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("statistics")]
    [RequireRole("admin", "manager", "employee")]
    public async Task<IActionResult> GetPaymentStatistics([FromQuery] GetPaymentStatisticsCommand command)
        => CreateActionResult(await _mediator.Send(command));
}


