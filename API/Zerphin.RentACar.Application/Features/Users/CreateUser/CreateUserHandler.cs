using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;
using Zerphin.RentACar.Domain.Contracts.Services;
using Zerphin.RentACar.Domain.Entities;
using Zerphin.RentACar.Domain.Exceptions;

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
        try
        {
            // Check if email already exists
            if (await _userRepository.IsEmailExistsAsync(request.Email))
            {
                throw new ConflictException($"User with email '{request.Email}' already exists.");
            }

            // Check if identity number already exists
            if (!string.IsNullOrEmpty(request.IdentityNumber) && await _userRepository.IsIdentityNumberExistsAsync(request.IdentityNumber))
            {
                throw new ConflictException($"User with identity number '{request.IdentityNumber}' already exists.");
            }

            // Check if role exists
            var role = await _roleRepository.GetByIdAsync(request.RoleId);
            if (role == null)
            {
                throw new NotFoundException(nameof(Role), request.RoleId);
            }

            // Validate password strength
            if (!_passwordService.IsPasswordStrong(request.Password))
            {
                throw new ValidationException("Password does not meet security requirements.");
            }

            // Create user entity
            var user = _mapper.Map<User>(request);
            user.PasswordHash = _passwordService.HashPassword(request.Password);
            user.IsActive = true;
            
            var createdUser = await _userRepository.AddAsync(user);

            await _unitOfWork.SaveChangesAsync();

            // Map to response
            var response = _mapper.Map<CreateUserResponse>(createdUser);

            return ServiceResult<CreateUserResponse>.Success(response, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            throw new BusinessException("An error occurred while creating the user.", ex);
        }
    }
}
