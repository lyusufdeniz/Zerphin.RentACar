using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zerphin.RentACar.Application.Features.Rentals.CreateRental;
using Zerphin.RentACar.Application.Features.Rentals.DeleteRental;
using Zerphin.RentACar.Application.Features.Rentals.GetRentalById;
using Zerphin.RentACar.Application.Features.Rentals.SearchRentals;
using Zerphin.RentACar.Application.Features.Rentals.UpdateRental;
using Zerphin.RentACar.Application.Features.Rentals.UpdateRentalStatus;
using Zerphin.RentACar.Domain.Attributes;

namespace Zerphin.RentACar.API.Controllers;

public class RentalsController : BaseController
{
    public RentalsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [RequireRole("admin", "manager", "employee")]
    public async Task<IActionResult> CreateRental([FromBody] CreateRentalCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("id/")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> GetRentalById([FromQuery] GetRentalByIdCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("search")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> SearchRentals([FromQuery] SearchRentalsCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpPut]
    [RequireRole("admin", "manager", "employee")]
    public async Task<IActionResult> UpdateRental([FromBody] UpdateRentalCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpDelete]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> DeleteRental([FromQuery] DeleteRentalCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpPatch]
    [RequireRole("admin", "manager", "employee")]
    public async Task<IActionResult> UpdateRentalStatus([FromBody] UpdateRentalStatusCommand command)
        => CreateActionResult(await _mediator.Send(command));
}

