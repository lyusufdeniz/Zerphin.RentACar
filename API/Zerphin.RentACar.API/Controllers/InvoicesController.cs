using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zerphin.RentACar.Application.Features.Invoices.CreateInvoice;
using Zerphin.RentACar.Application.Features.Invoices.DeleteInvoice;
using Zerphin.RentACar.Application.Features.Invoices.GetInvoiceById;
using Zerphin.RentACar.Application.Features.Invoices.SearchInvoices;
using Zerphin.RentACar.Application.Features.Invoices.UpdateInvoice;
using Zerphin.RentACar.Domain.Attributes;

namespace Zerphin.RentACar.API.Controllers;

public class InvoicesController : BaseController
{
    public InvoicesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [RequireRole("admin", "manager", "employee")]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("id/")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> GetInvoiceById([FromQuery] GetInvoiceByIdCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("search")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> SearchInvoices([FromQuery] SearchInvoicesCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpPut]
    [RequireRole("admin", "manager", "employee")]
    public async Task<IActionResult> UpdateInvoice([FromBody] UpdateInvoiceCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpDelete]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> DeleteInvoice([FromQuery] DeleteInvoiceCommand command)
        => CreateActionResult(await _mediator.Send(command));
}

