using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Users.ActivateUser;

public class ActivateUserHandler : IRequestHandler<ActivateUserCommand, ServiceResult<ActivateUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateUserHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<ActivateUserResponse>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var success = await _userRepository.ActivateUserAsync(request.Id);
        if (!success)
        {
            return ServiceResult<ActivateUserResponse>.Fail($"User with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        await _unitOfWork.SaveChangesAsync();
        return ServiceResult<ActivateUserResponse>.Success(new ActivateUserResponse());
    }
}

