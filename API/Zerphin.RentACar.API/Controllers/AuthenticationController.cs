using Zerphin.RentACar.Application.Features.Authentication.Login;
using Zerphin.RentACar.Application.Features.Authentication.RT;
using Zerphin.RentACar.Application.Features.Authentication.Register;
using Zerphin.RentACar.Domain.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Zerphin.RentACar.API.Controllers;

public class AuthenticationController : BaseController
{
    public AuthenticationController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command) => CreateActionResult(await _mediator.Send(command));

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command) => CreateActionResult(await _mediator.Send(command));

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command) => CreateActionResult(await _mediator.Send(command));
}
