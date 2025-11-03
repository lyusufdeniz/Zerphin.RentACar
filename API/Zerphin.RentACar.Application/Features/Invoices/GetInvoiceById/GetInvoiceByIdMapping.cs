using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Invoices.GetInvoiceById;

public class GetInvoiceByIdMapping : Profile
{
    public GetInvoiceByIdMapping()
    {
        CreateMap<Invoice, GetInvoiceByIdResponse>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Rental != null ? src.Rental.CustomerId : (Guid?)null))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Rental != null && src.Rental.Customer != null ? src.Rental.Customer.Email : null));
    }
}



