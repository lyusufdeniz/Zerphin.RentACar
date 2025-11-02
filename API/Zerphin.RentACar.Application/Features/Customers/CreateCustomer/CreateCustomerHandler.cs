using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Customers.CreateCustomer;

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, ServiceResult<CreateCustomerResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IRepository<Domain.Entities.User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCustomerHandler(
        ICustomerRepository customerRepository,
        IRepository<Domain.Entities.User> userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<CreateCustomerResponse>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Check if license number already exists
        if (await _customerRepository.IsLicenseNumberExistsAsync(request.LicenseNumber))
        {
            return ServiceResult<CreateCustomerResponse>.Fail($"Customer with license number '{request.LicenseNumber}' already exists.", HttpStatusCode.Conflict);
        }

        // Check if user exists
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return ServiceResult<CreateCustomerResponse>.Fail($"User with ID {request.UserId} not found.", HttpStatusCode.NotFound);
        }

        // Check if user already has a customer record
        var existingCustomer = await _customerRepository.GetByUserIdAsync(request.UserId);
        if (existingCustomer != null)
        {
            return ServiceResult<CreateCustomerResponse>.Fail($"User with ID {request.UserId} already has a customer record.", HttpStatusCode.Conflict);
        }

        // Create customer entity
        var customer = _mapper.Map<Domain.Entities.Customer>(request);
        var createdCustomer = await _customerRepository.AddAsync(customer);

        await _unitOfWork.SaveChangesAsync();

        // Reload customer
        createdCustomer = await _customerRepository.GetByIdAsync(createdCustomer.Id);

        // Map to response
        var response = _mapper.Map<CreateCustomerResponse>(createdCustomer);

        return ServiceResult<CreateCustomerResponse>.Success(response, HttpStatusCode.Created);
    }
}


