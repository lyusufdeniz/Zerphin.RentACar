using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Insurances.UpdateInsurance;

public class UpdateInsuranceMapping : Profile
{
    public UpdateInsuranceMapping()
    {
        CreateMap<UpdateInsuranceCommand, Insurance>()
            .ForMember(dest => dest.VehicleId, opt => opt.Ignore())
            .ForMember(dest => dest.Vehicle, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        CreateMap<Insurance, UpdateInsuranceResponse>()
            .ForMember(dest => dest.VehicleBrand, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Brand : null))
            .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Model : null))
            .ForMember(dest => dest.VehicleLicensePlate, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.LicensePlate : null));
    }
}



