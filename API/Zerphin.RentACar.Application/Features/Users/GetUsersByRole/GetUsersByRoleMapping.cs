using AutoMapper;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Users.GetUsersByRole;

public class GetUsersByRoleMapping : Profile
{
    public GetUsersByRoleMapping()
    {
        CreateMap<User, GetUsersByRoleResponse.UserDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : string.Empty));
    }
}

