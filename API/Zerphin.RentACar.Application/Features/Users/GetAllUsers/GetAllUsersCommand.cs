using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Users.GetAllUsers;

public class GetAllUsersCommand : IRequest<ServiceResult<GetAllUsersResponse>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? OrderBy { get; set; } = "Id";
    public bool IsDescending { get; set; } = false;
}

