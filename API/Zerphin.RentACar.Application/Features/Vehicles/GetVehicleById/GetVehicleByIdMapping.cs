using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Vehicles.GetVehicleById;

public class GetVehicleByIdMapping : Profile
{
    public GetVehicleByIdMapping()
    {
        CreateMap<Vehicle, GetVehicleByIdResponse>();
    }
}

