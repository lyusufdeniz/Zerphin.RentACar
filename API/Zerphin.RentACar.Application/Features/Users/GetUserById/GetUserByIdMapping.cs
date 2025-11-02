using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Users.GetUserById;

public class GetUserByIdMapping : Profile
{
    public GetUserByIdMapping()
    {
        CreateMap<User, GetUserByIdResponse>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.ToString()));
    }
}

