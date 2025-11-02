using MediatR;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.Application.Features.Vehicles.GetVehicleById;

public class GetVehicleByIdCommand : IRequest<ServiceResult<GetVehicleByIdResponse>>
{
    public Guid Id { get; set; }
}

