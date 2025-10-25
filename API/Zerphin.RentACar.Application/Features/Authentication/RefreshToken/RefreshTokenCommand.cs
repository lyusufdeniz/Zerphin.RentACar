using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Authentication.RT;

public class RefreshTokenCommand : IRequest<ServiceResult<RefreshTokenResponse>>
{
    public string RefreshToken { get; set; } = string.Empty;
}
