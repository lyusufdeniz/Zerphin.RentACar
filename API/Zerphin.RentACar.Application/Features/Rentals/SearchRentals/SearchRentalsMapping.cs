using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Rentals.SearchRentals;

public class SearchRentalsMapping : Profile
{
    public SearchRentalsMapping()
    {
        CreateMap<Rental, SearchRentalsResponse.RentalDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : null))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Email : null))
            .ForMember(dest => dest.VehicleBrand, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Brand : null))
            .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.Model : null))
            .ForMember(dest => dest.VehicleLicensePlate, opt => opt.MapFrom(src => src.Vehicle != null ? src.Vehicle.LicensePlate : null));
    }
}

