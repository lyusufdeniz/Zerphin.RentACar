using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Customers.GetCustomerById;

public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdCommand, ServiceResult<GetCustomerByIdResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public GetCustomerByIdHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<GetCustomerByIdResponse>> Handle(GetCustomerByIdCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);
        
        if (customer == null)
        {
            return ServiceResult<GetCustomerByIdResponse>.Fail($"Customer with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        var response = _mapper.Map<GetCustomerByIdResponse>(customer);
        return ServiceResult<GetCustomerByIdResponse>.Success(response);
    }
}

