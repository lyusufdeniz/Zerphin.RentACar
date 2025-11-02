using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Customers.SearchCustomers;

public class SearchCustomersMapping : Profile
{
    public SearchCustomersMapping()
    {
        CreateMap<Customer, SearchCustomersResponse.CustomerDto>()
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : null));
    }
}


