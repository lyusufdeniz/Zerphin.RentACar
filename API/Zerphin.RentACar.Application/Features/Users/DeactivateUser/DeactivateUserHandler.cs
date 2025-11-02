using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Users.DeactivateUser;

public class DeactivateUserHandler : IRequestHandler<DeactivateUserCommand, ServiceResult<DeactivateUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateUserHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<DeactivateUserResponse>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var success = await _userRepository.DeactivateUserAsync(request.Id);
        if (!success)
        {
            return ServiceResult<DeactivateUserResponse>.Fail($"User with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        await _unitOfWork.SaveChangesAsync();
        return ServiceResult<DeactivateUserResponse>.Success(new DeactivateUserResponse());
    }
}

