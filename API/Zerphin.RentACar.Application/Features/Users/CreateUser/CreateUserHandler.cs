using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Contracts.Services;
using Zerphin.RentACar.Domain.Entities;

namespace Zerphin.RentACar.Application.Features.Users.CreateUser;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, ServiceResult<CreateUserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordService _passwordService;

    public CreateUserHandler(IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork, IMapper mapper, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordService = passwordService;
    }

    public async Task<ServiceResult<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if email already exists
        if (await _userRepository.IsEmailExistsAsync(request.Email))
        {
            return ServiceResult<CreateUserResponse>.Fail($"User with email '{request.Email}' already exists.", HttpStatusCode.Conflict);
        }

        // Check if identity number already exists
        if (!string.IsNullOrEmpty(request.IdentityNumber) && await _userRepository.IsIdentityNumberExistsAsync(request.IdentityNumber))
        {
            return ServiceResult<CreateUserResponse>.Fail($"User with identity number '{request.IdentityNumber}' already exists.", HttpStatusCode.Conflict);
        }

        // Check if role exists
        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role == null)
        {
            return ServiceResult<CreateUserResponse>.Fail($"Role with ID {request.RoleId} not found.", HttpStatusCode.NotFound);
        }

        // Validate password strength
        if (!_passwordService.IsPasswordStrong(request.Password))
        {
            return ServiceResult<CreateUserResponse>.Fail("Password does not meet security requirements.", HttpStatusCode.BadRequest);
        }

        // Create user entity
        var user = _mapper.Map<User>(request);
        user.PasswordHash = _passwordService.HashPassword(request.Password);
        user.IsActive = true;
        
        var createdUser = await _userRepository.AddAsync(user);

        await _unitOfWork.SaveChangesAsync();

        // Map to response
        var response = _mapper.Map<CreateUserResponse>(createdUser);

        return ServiceResult<CreateUserResponse>.Success(response, HttpStatusCode.Created);
    }
}
