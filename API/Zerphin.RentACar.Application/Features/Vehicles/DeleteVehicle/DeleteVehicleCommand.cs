using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Vehicles.DeleteVehicle;

public class DeleteVehicleCommand : IRequest<ServiceResult<DeleteVehicleResponse>>
{
    public Guid Id { get; set; }
}

