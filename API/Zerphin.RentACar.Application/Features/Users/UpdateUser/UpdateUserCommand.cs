using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Users.UpdateUser;

public class UpdateUserCommand : IRequest<ServiceResult<UpdateUserResponse>>
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string? Address { get; set; }
    public string? IdentityNumber { get; set; }
    public DateTime? BirthDate { get; set; }
}

