using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Invoices.UpdateInvoice;

public class UpdateInvoiceMapping : Profile
{
    public UpdateInvoiceMapping()
    {
        CreateMap<UpdateInvoiceCommand, Invoice>()
            .ForMember(dest => dest.RentalId, opt => opt.Ignore())
            .ForMember(dest => dest.Rental, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        CreateMap<Invoice, UpdateInvoiceResponse>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Rental != null ? src.Rental.CustomerId : (Guid?)null))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Rental != null && src.Rental.Customer != null ? src.Rental.Customer.Email : null));
    }
}



