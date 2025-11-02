using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Users.UpdateUser;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, ServiceResult<UpdateUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<UpdateUserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user == null || user.IsDeleted)
        {
            return ServiceResult<UpdateUserResponse>.Fail($"User with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        // Check if email already exists for another user
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null && existingUser.Id != request.Id)
        {
            return ServiceResult<UpdateUserResponse>.Fail($"User with email '{request.Email}' already exists.", HttpStatusCode.Conflict);
        }

        // Check if identity number already exists for another user
        if (!string.IsNullOrEmpty(request.IdentityNumber))
        {
            var existingUserByIdentity = await _userRepository.GetByIdentityNumberAsync(request.IdentityNumber);
            if (existingUserByIdentity != null && existingUserByIdentity.Id != request.Id)
            {
                return ServiceResult<UpdateUserResponse>.Fail($"User with identity number '{request.IdentityNumber}' already exists.", HttpStatusCode.Conflict);
            }
        }

        _mapper.Map(request, user);
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var response = _mapper.Map<UpdateUserResponse>(user);
        return ServiceResult<UpdateUserResponse>.Success(response);
    }
}

