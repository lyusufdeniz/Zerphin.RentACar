using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Contracts.Services;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.Options;

namespace Zerphin.RentACar.Application.Features.Authentication.Login;

public class LoginHandler : IRequestHandler<LoginCommand, ServiceResult<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRepository<Domain.Entities.RefreshToken> _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;
    private readonly JwtOptions _jwtOptions;
    private readonly IPasswordService _passwordService;

    public LoginHandler(IUserRepository userRepository, IRepository<RefreshToken> refreshTokenRepository, IUnitOfWork unitOfWork, IAuthenticationService authenticationService, IMapper mapper, IOptions<JwtOptions> jwtOptions, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _authenticationService = authenticationService;
        _mapper = mapper;
        _jwtOptions = jwtOptions.Value;
        _passwordService = passwordService;
    }

    public async Task<ServiceResult<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Get user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            return ServiceResult<LoginResponse>.Fail("Invalid email or password.", HttpStatusCode.Unauthorized);
        }

        if (!user.IsActive)
        {
            return ServiceResult<LoginResponse>.Fail("User account is deactivated.", HttpStatusCode.Unauthorized);
        }

        // Verify password
        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return ServiceResult<LoginResponse>.Fail("Invalid email or password.", HttpStatusCode.Unauthorized);
        }

        // Generate JWT token
        var token = await _authenticationService.GenerateJwtTokenAsync(user);

        // Generate refresh token
        var refreshToken = await _authenticationService.GenerateRefreshTokenAsync();

        // Save refresh token to database
        var refreshTokenEntity = new Domain.Entities.RefreshToken
        {
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
            UserId = user.Id
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        await _unitOfWork.SaveChangesAsync();

        // Map to response using AutoMapper
        var response = _mapper.Map<LoginResponse>(user);
        response.Token = token;
        response.RefreshToken = refreshToken;
        response.ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes);

        return ServiceResult<LoginResponse>.Success(response);
    }
}
