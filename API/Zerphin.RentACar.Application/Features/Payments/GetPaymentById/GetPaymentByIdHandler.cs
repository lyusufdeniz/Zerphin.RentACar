using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Payments.GetPaymentById;

public class GetPaymentByIdHandler : IRequestHandler<GetPaymentByIdCommand, ServiceResult<GetPaymentByIdResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentByIdHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<GetPaymentByIdResponse>> Handle(GetPaymentByIdCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.Id);
        
        if (payment == null)
        {
            return ServiceResult<GetPaymentByIdResponse>.Fail($"Payment with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        var response = _mapper.Map<GetPaymentByIdResponse>(payment);
        return ServiceResult<GetPaymentByIdResponse>.Success(response);
    }
}


