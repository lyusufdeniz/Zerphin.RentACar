using AutoMapper;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.ValueObjects;

namespace Zerphin.RentACar.Application.Features.Vehicles.CreateVehicle;

public class CreateVehicleMapping : Profile
{
    public CreateVehicleMapping()
    {
        CreateMap<CreateVehicleCommand, Vehicle>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => VehicleStatus.Available)) // Status is always Available by default, managed by system
            .ForMember(dest => dest.Rentals, opt => opt.Ignore())
            .ForMember(dest => dest.Insurance, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        CreateMap<Vehicle, CreateVehicleResponse>();
    }
}

