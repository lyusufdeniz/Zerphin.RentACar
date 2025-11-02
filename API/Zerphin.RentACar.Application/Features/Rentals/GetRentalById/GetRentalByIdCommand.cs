using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Rentals.GetRentalById;

public class GetRentalByIdCommand : IRequest<ServiceResult<GetRentalByIdResponse>>
{
    public Guid Id { get; set; }
}


