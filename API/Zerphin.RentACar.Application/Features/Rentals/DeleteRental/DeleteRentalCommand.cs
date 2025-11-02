using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Rentals.DeleteRental;

public class DeleteRentalCommand : IRequest<ServiceResult<DeleteRentalResponse>>
{
    public Guid Id { get; set; }
}

