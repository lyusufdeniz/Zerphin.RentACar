using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Users.GetUserById;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdCommand, ServiceResult<GetUserByIdResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<GetUserByIdResponse>> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        
        if (user == null || user.IsDeleted)
        {
            return ServiceResult<GetUserByIdResponse>.Fail($"User with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        var response = _mapper.Map<GetUserByIdResponse>(user);
        return ServiceResult<GetUserByIdResponse>.Success(response);
    }
}

