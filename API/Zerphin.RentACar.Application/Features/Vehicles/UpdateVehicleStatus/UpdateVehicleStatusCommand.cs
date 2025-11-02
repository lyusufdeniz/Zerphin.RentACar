using MediatR;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Vehicles.UpdateVehicleStatus;

public class UpdateVehicleStatusCommand : IRequest<ServiceResult<UpdateVehicleStatusResponse>>
{
    public Guid Id { get; set; }
    public VehicleStatus Status { get; set; }
}


