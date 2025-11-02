using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Contracts.Services;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Users.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, ServiceResult<ChangePasswordResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;

    public ChangePasswordHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
    }

    public async Task<ServiceResult<ChangePasswordResponse>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // Check if user exists
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return ServiceResult<ChangePasswordResponse>.Fail($"User with ID {request.UserId} not found.", HttpStatusCode.NotFound);
        }

        // Verify current password
        if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            return ServiceResult<ChangePasswordResponse>.Fail("Current password is incorrect.", HttpStatusCode.Unauthorized);
        }

        // Validate new password strength
        if (!_passwordService.IsPasswordStrong(request.NewPassword))
        {
            return ServiceResult<ChangePasswordResponse>.Fail("New password does not meet security requirements.", HttpStatusCode.BadRequest);
        }

        // Update password
        user.PasswordHash = _passwordService.HashPassword(request.NewPassword);
        await _userRepository.UpdateAsync(user);

        await _unitOfWork.SaveChangesAsync();

        var response = new ChangePasswordResponse
        {
            Message = "Password changed successfully."
        };

        return ServiceResult<ChangePasswordResponse>.Success(response);
    }
}
