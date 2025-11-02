using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Vehicles.SearchVehicles;

public class SearchVehiclesMapping : Profile
{
    public SearchVehiclesMapping()
    {
        CreateMap<Vehicle, SearchVehiclesResponse.VehicleDto>();
    }
}

