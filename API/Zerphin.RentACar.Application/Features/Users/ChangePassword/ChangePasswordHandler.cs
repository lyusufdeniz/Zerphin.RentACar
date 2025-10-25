using MediatR;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Contracts.Services;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.Exceptions;

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
        try
        {
            // Check if user exists
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new NotFoundException(nameof(User), request.UserId);
            }

            // Verify current password
            if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                throw new UnauthorizedException("Current password is incorrect.");
            }

            // Validate new password strength
            if (!_passwordService.IsPasswordStrong(request.NewPassword))
            {
                throw new ValidationException("New password does not meet security requirements.");
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
        catch (Exception ex)
        {
            throw new BusinessException("An error occurred while changing the password.", ex);
        }
    }
}
