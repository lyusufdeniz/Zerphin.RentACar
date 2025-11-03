using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Users.SearchUsers;

public class SearchUsersMapping : Profile
{
    public SearchUsersMapping()
    {
        CreateMap<User, SearchUsersResponse.UserDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.ToString()));
    }
}

