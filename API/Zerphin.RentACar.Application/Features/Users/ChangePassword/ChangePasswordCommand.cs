using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Users.ChangePassword;

public class ChangePasswordCommand : IRequest<ServiceResult<ChangePasswordResponse>>
{
    public int UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
