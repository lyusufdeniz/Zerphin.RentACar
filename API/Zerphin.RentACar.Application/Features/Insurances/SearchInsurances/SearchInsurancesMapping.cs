using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Insurances.SearchInsurances;

public class SearchInsurancesMapping : Profile
{
    public SearchInsurancesMapping()
    {
        CreateMap<Insurance, SearchInsurancesResponse.InsuranceDto>()
            .ForMember(dest => dest.VehicleBrand, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Brand : null))
            .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Model : null))
            .ForMember(dest => dest.VehicleLicensePlate, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.LicensePlate : null));
    }
}



