using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zerphin.RentACar.Application.Features.Insurances.CreateInsurance;
using Zerphin.RentACar.Application.Features.Insurances.DeleteInsurance;
using Zerphin.RentACar.Application.Features.Insurances.GetInsuranceById;
using Zerphin.RentACar.Application.Features.Insurances.SearchInsurances;
using Zerphin.RentACar.Application.Features.Insurances.UpdateInsurance;
using Zerphin.RentACar.Domain.Attributes;

namespace Zerphin.RentACar.API.Controllers;

public class InsurancesController : BaseController
{
    public InsurancesController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> CreateInsurance([FromBody] CreateInsuranceCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("id/")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> GetInsuranceById([FromQuery] GetInsuranceByIdCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("search")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> SearchInsurances([FromQuery] SearchInsurancesCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpPut]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> UpdateInsurance([FromBody] UpdateInsuranceCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpDelete]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> DeleteInsurance([FromQuery] DeleteInsuranceCommand command)
        => CreateActionResult(await _mediator.Send(command));
}

