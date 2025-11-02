using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Users.GetAllUsers;

public class GetAllUsersMapping : Profile
{
    public GetAllUsersMapping()
    {
        CreateMap<User, GetAllUsersResponse.UserDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.ToString()));
    }
}

