using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Users.DeleteUser;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, ServiceResult<DeleteUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<DeleteUserResponse>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        if (user == null || user.IsDeleted)
        {
            return ServiceResult<DeleteUserResponse>.Fail($"User with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        await _userRepository.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<DeleteUserResponse>.Success(new DeleteUserResponse());
    }
}

