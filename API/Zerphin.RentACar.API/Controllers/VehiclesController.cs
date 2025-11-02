using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zerphin.RentACar.Application.Features.Vehicles.CreateVehicle;
using Zerphin.RentACar.Application.Features.Vehicles.DeleteVehicle;
using Zerphin.RentACar.Application.Features.Vehicles.GetVehicleById;
using Zerphin.RentACar.Application.Features.Vehicles.SearchVehicles;
using Zerphin.RentACar.Application.Features.Vehicles.UpdateVehicle;
using Zerphin.RentACar.Application.Features.Vehicles.UpdateVehicleStatus;
using Zerphin.RentACar.Application.Features.Vehicles.GetVehicleStatistics;
using Zerphin.RentACar.Domain.Attributes;

namespace Zerphin.RentACar.API.Controllers;

public class VehiclesController : BaseController
{
    public VehiclesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleCommand command) 
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("id/")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> GetVehicleById([FromQuery] GetVehicleByIdCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("search")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> SearchVehicles([FromQuery] SearchVehiclesCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpPut]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> UpdateVehicle([FromBody] UpdateVehicleCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpDelete]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> DeleteVehicle([FromQuery] DeleteVehicleCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpPatch]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> UpdateVehicleStatus([FromBody] UpdateVehicleStatusCommand command) => CreateActionResult(await _mediator.Send(command));

    [HttpGet("statistics")]
    [RequireRole("admin", "manager", "employee")]
    public async Task<IActionResult> GetVehicleStatistics([FromQuery] GetVehicleStatisticsCommand command)
        => CreateActionResult(await _mediator.Send(command));
}
