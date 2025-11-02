using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Customers.UpdateCustomer;

public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, ServiceResult<UpdateCustomerResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCustomerHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<UpdateCustomerResponse>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);
        if (customer == null)
        {
            return ServiceResult<UpdateCustomerResponse>.Fail($"Customer with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        // Check if license number is being changed and if new one already exists
        if (customer.LicenseNumber != request.LicenseNumber)
        {
            if (await _customerRepository.IsLicenseNumberExistsAsync(request.LicenseNumber))
            {
                return ServiceResult<UpdateCustomerResponse>.Fail($"Customer with license number '{request.LicenseNumber}' already exists.", HttpStatusCode.Conflict);
            }
        }

        // Update customer properties
        _mapper.Map(request, customer);
        await _customerRepository.UpdateAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        // Reload customer
        customer = await _customerRepository.GetByIdAsync(customer.Id);

        var response = _mapper.Map<UpdateCustomerResponse>(customer);
        return ServiceResult<UpdateCustomerResponse>.Success(response);
    }
}

