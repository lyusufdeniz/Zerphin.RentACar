using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Invoices.SearchInvoices;

public class SearchInvoicesMapping : Profile
{
    public SearchInvoicesMapping()
    {
        CreateMap<Invoice, SearchInvoicesResponse.InvoiceDto>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Rental != null ? src.Rental.CustomerId : (Guid?)null))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Rental != null && src.Rental.Customer != null ? src.Rental.Customer.Email : null));
    }
}



