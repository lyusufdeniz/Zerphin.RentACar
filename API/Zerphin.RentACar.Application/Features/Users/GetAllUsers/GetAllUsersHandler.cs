using AutoMapper;
using MediatR;
using System.Linq.Expressions;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Users.GetAllUsers;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersCommand, ServiceResult<GetAllUsersResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<GetAllUsersResponse>> Handle(GetAllUsersCommand request, CancellationToken cancellationToken)
    {
        Expression<Func<User, object>> orderBy = request.OrderBy?.ToLower() switch
        {
            "firstname" => x => x.FirstName,
            "lastname" => x => x.LastName,
            "email" => x => x.Email,
            "createdat" => x => x.CreatedAt,
            _ => x => x.Id
        };

        var pagedResult = await _userRepository.GetPagedUsersAsync(
            request.PageNumber,
            request.PageSize,
            orderBy,
            request.IsDescending
        );

        var response = new GetAllUsersResponse
        {
            Users = _mapper.Map<List<GetAllUsersResponse.UserDto>>(pagedResult.Data),
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize
        };

        return ServiceResult<GetAllUsersResponse>.Success(response);
    }
}

