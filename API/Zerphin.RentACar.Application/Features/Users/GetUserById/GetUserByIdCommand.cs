using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Users.GetUserById;

public class GetUserByIdCommand : IRequest<ServiceResult<GetUserByIdResponse>>
{
    public Guid Id { get; set; }
}

