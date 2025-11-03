using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zerphin.RentACar.Application.Features.Users.ActivateUser;
using Zerphin.RentACar.Application.Features.Users.ChangePassword;
using Zerphin.RentACar.Application.Features.Users.CreateUser;
using Zerphin.RentACar.Application.Features.Users.DeactivateUser;
using Zerphin.RentACar.Application.Features.Users.DeleteUser;
using Zerphin.RentACar.Application.Features.Users.GetAllUsers;
using Zerphin.RentACar.Application.Features.Users.GetUserById;
using Zerphin.RentACar.Application.Features.Users.GetUserStatistics;
using Zerphin.RentACar.Application.Features.Users.GetUsersByRole;
using Zerphin.RentACar.Application.Features.Users.SearchUsers;
using Zerphin.RentACar.Application.Features.Users.UpdateUser;
using Zerphin.RentACar.Domain.Attributes;

namespace Zerphin.RentACar.API.Controllers;

public class UsersController : BaseController
{
    public UsersController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> CreateUser(CreateUserCommand command) => CreateActionResult(await _mediator.Send(command));

    [HttpPut("change-password")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command) => CreateActionResult(await _mediator.Send(command));

    [HttpGet("statistics")]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> GetUserStatistics([FromQuery] GetUserStatisticsCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("search")]
    [RequireRole("admin", "manager", "employee")]
    public async Task<IActionResult> SearchUsers([FromQuery] SearchUsersCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("{id}")]
    [RequireRole("admin", "manager", "employee", "customer")]
    public async Task<IActionResult> GetUserById(Guid id) 
        => CreateActionResult(await _mediator.Send(new GetUserByIdCommand { Id = id }));

    [HttpGet]
    [RequireRole("admin", "manager", "employee")]
    public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpGet("role")]
    [RequireRole("admin", "manager", "employee")]
    public async Task<IActionResult> GetUsersByRole([FromQuery] GetUsersByRoleCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpPut]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpDelete]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> DeleteUser([FromQuery] DeleteUserCommand command)
        => CreateActionResult(await _mediator.Send(command));

    [HttpPatch("{id}/activate")]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> ActivateUser(Guid id)
        => CreateActionResult(await _mediator.Send(new ActivateUserCommand { Id = id }));

    [HttpPatch("{id}/deactivate")]
    [RequireRole("admin", "manager")]
    public async Task<IActionResult> DeactivateUser(Guid id)
        => CreateActionResult(await _mediator.Send(new DeactivateUserCommand { Id = id }));
}
