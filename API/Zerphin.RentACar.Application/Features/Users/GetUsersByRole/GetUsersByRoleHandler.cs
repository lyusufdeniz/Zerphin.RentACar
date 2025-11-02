using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Users.GetUsersByRole;

public class GetUsersByRoleHandler : IRequestHandler<GetUsersByRoleCommand, ServiceResult<GetUsersByRoleResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersByRoleHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<GetUsersByRoleResponse>> Handle(GetUsersByRoleCommand request, CancellationToken cancellationToken)
    {
        var pagedResult = await _userRepository.GetPagedByRoleAsync(
            request.RoleName,
            request.PageNumber,
            request.PageSize
        );

        var response = new GetUsersByRoleResponse
        {
            Users = _mapper.Map<List<GetUsersByRoleResponse.UserDto>>(pagedResult.Data),
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize
        };

        return ServiceResult<GetUsersByRoleResponse>.Success(response);
    }
}

