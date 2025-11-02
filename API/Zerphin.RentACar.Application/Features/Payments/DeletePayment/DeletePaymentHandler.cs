using MediatR;
using System.Net;
using Zerphin.RentACar.Application.Common;
using Zerphin.RentACar.Domain.Contracts.Repositories;

namespace Zerphin.RentACar.Application.Features.Payments.DeletePayment;

public class DeletePaymentHandler : IRequestHandler<DeletePaymentCommand, ServiceResult<DeletePaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePaymentHandler(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<DeletePaymentResponse>> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.Id);
        if (payment == null)
        {
            return ServiceResult<DeletePaymentResponse>.Fail($"Payment with ID {request.Id} not found.", HttpStatusCode.NotFound);
        }

        // Soft delete payment
        await _paymentRepository.DeleteAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        var response = new DeletePaymentResponse
        {
            Id = request.Id,
            Message = "Payment deleted successfully."
        };

        return ServiceResult<DeletePaymentResponse>.Success(response);
    }
}


