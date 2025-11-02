using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zerphin.RentACar.Application.Features.Customers.CreateCustomer;
using Zerphin.RentACar.Application.Features.Customers.DeleteCustomer;
using Zerphin.RentACar.Application.Features.Customers.GetCustomerById;
using Zerphin.RentACar.Application.Features.Customers.SearchCustomers;
using Zerphin.RentACar.Application.Features.Customers.UpdateCustomer;
using Zerphin.RentACar.Application.Features.Customers.GetCustomerStatistics;
using Zerphin.RentACar.Domain.Attributes;

namespace Zerphin.RentACar.API.Controllers;

public class CustomersController : BaseController
{
    public CustomersController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("id/")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> GetCustomerById([FromQuery] GetCustomerByIdCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("search")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> SearchCustomers([FromQuery] SearchCustomersCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpPut]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpDelete]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> DeleteCustomer([FromQuery] DeleteCustomerCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("statistics")]
    [RequireRole("admin", "manager", "employee")]
    public async Task<IActionResult> GetCustomerStatistics([FromQuery] GetCustomerStatisticsCommand command)
        => CreateActionResult(await _mediator.Send(command));
}


