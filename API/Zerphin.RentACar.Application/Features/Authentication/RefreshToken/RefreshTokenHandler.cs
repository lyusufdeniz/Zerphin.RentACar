using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Contracts.Services;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.Options;

namespace Zerphin.RentACar.Application.Features.Authentication.RT;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, ServiceResult<RefreshTokenResponse>>
{
    private readonly IRepository<Domain.Entities.RefreshToken> _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticationService _authenticationService;
    private readonly JwtOptions _jwtOptions;

    public RefreshTokenHandler(IRepository<Domain.Entities.RefreshToken> refreshTokenRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IAuthenticationService authenticationService, JwtOptions jwtOptions)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _authenticationService = authenticationService;
        _jwtOptions = jwtOptions;
    }

    public async Task<ServiceResult<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Find the refresh token
        var tokenEntity = await _refreshTokenRepository.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);
        if (tokenEntity == null)
        {
            return ServiceResult<RefreshTokenResponse>.Fail("Invalid refresh token.", HttpStatusCode.BadRequest);
        }

        // Check if the refresh token is expired
        if (tokenEntity.IsExpired)
        {
            // Remove expired token
            await _refreshTokenRepository.DeleteAsync(tokenEntity);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResult<RefreshTokenResponse>.Fail("Refresh token has expired.", HttpStatusCode.Unauthorized);
        }

        // Get the user
        var user = await _userRepository.GetByIdAsync(tokenEntity.UserId);
        if (user == null)
        {
            return ServiceResult<RefreshTokenResponse>.Fail($"User with ID {tokenEntity.UserId} not found.", HttpStatusCode.NotFound);
        }

        if (!user.IsActive)
        {
            return ServiceResult<RefreshTokenResponse>.Fail("User account is deactivated.", HttpStatusCode.Unauthorized);
        }

        // Generate new JWT token
        var newJwtToken = await _authenticationService.GenerateJwtTokenAsync(user);

        // Generate new refresh token
        var newRefreshToken = await _authenticationService.GenerateRefreshTokenAsync();

        // Update the refresh token in database
        tokenEntity.Token = newRefreshToken;
        tokenEntity.ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

        await _refreshTokenRepository.UpdateAsync(tokenEntity);

        await _unitOfWork.SaveChangesAsync();

        var response = new RefreshTokenResponse
        {
            Token = newJwtToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes)
        };

        return ServiceResult<RefreshTokenResponse>.Success(response);
    }
}
