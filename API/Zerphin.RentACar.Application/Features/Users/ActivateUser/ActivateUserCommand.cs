using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Users.ActivateUser;

public class ActivateUserCommand : IRequest<ServiceResult<ActivateUserResponse>>
{
    public Guid Id { get; set; }
}

