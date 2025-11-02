using AutoMapper;
using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Payments.UpdatePayment;

public class UpdatePaymentHandler : IRequestHandler<UpdatePaymentCommand, ServiceResult<UpdatePaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdatePaymentHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ServiceResult<UpdatePaymentResponse>> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.Id);
        if (payment == null)
        {
            return ServiceResult<UpdatePaymentResponse>.Fail($"Payment with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        // Update payment properties
        _mapper.Map(request, payment);
        await _paymentRepository.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        // Reload payment
        payment = await _paymentRepository.GetByIdAsync(payment.Id);

        var response = _mapper.Map<UpdatePaymentResponse>(payment);
        return ServiceResult<UpdatePaymentResponse>.Success(response);
    }
}


