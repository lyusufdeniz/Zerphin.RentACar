using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Customers.GetCustomerById;

public class GetCustomerByIdMapping : Profile
{
    public GetCustomerByIdMapping()
    {
        CreateMap<Customer, GetCustomerByIdResponse>()
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : null));
    }
}


