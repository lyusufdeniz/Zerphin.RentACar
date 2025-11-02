using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Customers.DeleteCustomer;

public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, ServiceResult<DeleteCustomerResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<DeleteCustomerResponse>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);
        if (customer == null)
        {
            return ServiceResult<DeleteCustomerResponse>.Fail($"Customer with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        // Soft delete customer
        await _customerRepository.DeleteAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        var response = new DeleteCustomerResponse
        {
            Id = request.Id,
            Message = "Customer deleted successfully."
        };

        return ServiceResult<DeleteCustomerResponse>.Success(response);
    }
}

