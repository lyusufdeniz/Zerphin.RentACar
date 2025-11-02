using MediatR;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Rentals.UpdateRentalStatus;

public class UpdateRentalStatusCommand : IRequest<ServiceResult<UpdateRentalStatusResponse>>
{
    public Guid Id { get; set; }
    public RentalStatus Status { get; set; }
}

