using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Payments.UpdatePayment;

public class UpdatePaymentMapping : Profile
{
    public UpdatePaymentMapping()
    {
        CreateMap<UpdatePaymentCommand, Payment>()
            .ForMember(dest => dest.RentalId, opt => opt.Ignore())
            .ForMember(dest => dest.Rental, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        CreateMap<Payment, UpdatePaymentResponse>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Rental != null ? src.Rental.CustomerId : (Guid?)null))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Rental != null && src.Rental.Customer != null ? $"{src.Rental.Customer.FirstName} {src.Rental.Customer.LastName}" : null));
    }
}


