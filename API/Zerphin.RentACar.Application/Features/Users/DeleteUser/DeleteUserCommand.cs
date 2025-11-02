using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Users.DeleteUser;

public class DeleteUserCommand : IRequest<ServiceResult<DeleteUserResponse>>
{
    public int Id { get; set; }
}

