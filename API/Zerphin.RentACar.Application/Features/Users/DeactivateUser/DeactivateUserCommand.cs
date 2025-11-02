using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Users.DeactivateUser;

public class DeactivateUserCommand : IRequest<ServiceResult<DeactivateUserResponse>>
{
    public Guid Id { get; set; }
}

