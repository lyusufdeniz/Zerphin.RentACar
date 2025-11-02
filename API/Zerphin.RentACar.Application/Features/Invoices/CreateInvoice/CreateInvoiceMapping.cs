using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Invoices.CreateInvoice;

public class CreateInvoiceMapping : Profile
{
    public CreateInvoiceMapping()
    {
        // Note: Invoice creation is handled manually in CreateInvoiceHandler
        // No direct mapping from Command to Invoice entity

        CreateMap<Invoice, CreateInvoiceResponse>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Rental != null ? src.Rental.CustomerId : (Guid?)null))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Rental != null && src.Rental.Customer != null ? src.Rental.Customer.Email : null));
    }
}

