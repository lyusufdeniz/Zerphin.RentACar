using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Payments.SearchPayments;

public class SearchPaymentsMapping : Profile
{
    public SearchPaymentsMapping()
    {
        CreateMap<Payment, SearchPaymentsResponse.PaymentDto>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Rental != null ? src.Rental.CustomerId : (Guid?)null))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Rental != null && src.Rental.Customer != null ? $"{src.Rental.Customer.FirstName} {src.Rental.Customer.LastName}" : null));
    }
}

