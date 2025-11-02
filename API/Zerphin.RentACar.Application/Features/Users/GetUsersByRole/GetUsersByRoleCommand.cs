using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Users.GetUsersByRole;

public class GetUsersByRoleCommand : IRequest<ServiceResult<GetUsersByRoleResponse>>
{
    public string RoleName { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

