using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zerphin.RentACar.Application.Features.Users.ChangePassword;
using Zerphin.RentACar.Application.Features.Users.CreateUser;
using Zerphin.RentACar.Domain.Attributes;

namespace Zerphin.RentACar.API.Controllers;

public class UsersController : BaseController
{
    public UsersController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command) => Ok(await _mediator.Send(command));

    [HttpPut("change-password")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command) => Ok(await _mediator.Send(command));
}
